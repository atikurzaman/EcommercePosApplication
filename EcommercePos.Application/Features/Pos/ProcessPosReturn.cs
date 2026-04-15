using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── ProcessPosReturn ───────────────────────────────────────────────────────────
public static class ProcessPosReturn
{
    public sealed record ReturnLineInput(
        Guid ProductId,
        Guid? VariantId,
        Guid? BatchId,
        decimal Quantity,
        decimal UnitPrice);

    public sealed record Request(
        Guid WarehouseId,
        Guid? CustomerId,
        Guid? OriginalSaleId,
        string? Notes,
        Guid CreatedByUserId,
        List<ReturnLineInput> Lines);

    public sealed record Response(Guid Id, string ReturnNo, decimal TotalAmount);

    public sealed record Command(Request Request);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var req = command.Request;

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == req.WarehouseId && !w.IsDeleted, ct);
            if (warehouse == null)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found"));

            if (req.Lines == null || req.Lines.Count == 0)
                return Result<Response>.Failure(Error.BadRequest("At least one return line is required"));

            // Validate original sale if provided
            if (req.OriginalSaleId.HasValue)
            {
                var sale = await _context.PosTransactions
                    .FirstOrDefaultAsync(t => t.Id == req.OriginalSaleId.Value && !t.IsDeleted, ct);
                if (sale == null)
                    return Result<Response>.Failure(Error.NotFound("Original sale transaction not found"));
            }

            var now = DateTime.Now;
            var returnId = Guid.NewGuid();
            var random4 = Random.Shared.Next(1000, 9999);
            var returnNo = $"RTN-{warehouse.Code}-{now:yyyyMMddHHmmss}-{random4}";

            decimal totalAmount = 0;
            var returnLines = new List<PosTransactionReturnLines>();

            foreach (var lineInput in req.Lines)
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == lineInput.ProductId, ct);
                if (product == null)
                    return Result<Response>.Failure(Error.NotFound($"Product {lineInput.ProductId} not found"));

                var lineTotal = lineInput.Quantity * lineInput.UnitPrice;

                returnLines.Add(new PosTransactionReturnLines
                {
                    Id = Guid.NewGuid(),
                    PosTransactionReturnId = returnId,
                    ProductId = lineInput.ProductId,
                    VariantId = lineInput.VariantId,
                    BatchId = lineInput.BatchId,
                    Quantity = lineInput.Quantity,
                    UnitPrice = lineInput.UnitPrice,
                    LineTotal = lineTotal,
                    CreatedAt = now,
                    CreatedBy = req.CreatedByUserId,
                    IsDeleted = false
                });

                totalAmount += lineTotal;

                // Add stock back (increment StockItems)
                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s =>
                        s.ProductId == lineInput.ProductId &&
                        s.WarehouseId == req.WarehouseId &&
                        !s.IsDeleted, ct);

                if (stockItem != null)
                {
                    stockItem.QuantityOnHand += lineInput.Quantity;
                    stockItem.LastUpdatedAt = now;

                    // Create StockMovement for return
                    _context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = lineInput.ProductId,
                        VariantId = lineInput.VariantId,
                        BatchId = lineInput.BatchId,
                        StockItemId = stockItem.Id,
                        ToWarehouseId = req.WarehouseId,
                        MovementTypeCode = "RETURN",
                        QuantityIn = lineInput.Quantity,
                        QuantityOut = 0,
                        BalanceAfter = stockItem.QuantityOnHand,
                        UnitCost = lineInput.UnitPrice,
                        ReferenceType = "POS_RETURN",
                        ReferenceId = returnId,
                        ReferenceNumber = returnNo,
                        Notes = $"POS return for {product.Name}",
                        OccurredAt = now,
                        CreatedAt = now,
                        CreatedBy = req.CreatedByUserId
                    });
                }
            }

            var posReturn = new PosTransactionReturns
            {
                Id = returnId,
                ReturnNo = returnNo,
                ReturnDate = now,
                WarehouseId = req.WarehouseId,
                CustomerId = req.CustomerId,
                SaleId = req.OriginalSaleId,
                TotalAmount = totalAmount,
                Notes = req.Notes,
                CreatedByUserId = req.CreatedByUserId,
                CreatedAt = now,
                CreatedBy = req.CreatedByUserId,
                IsDeleted = false
            };

            foreach (var line in returnLines)
                posReturn.PosTransactionReturnLines.Add(line);

            _context.PosTransactionReturns.Add(posReturn);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(posReturn.Id, posReturn.ReturnNo, posReturn.TotalAmount));
        }
    }
}
