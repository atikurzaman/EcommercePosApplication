using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── CreateHeldTransaction ──────────────────────────────────────────────────────
public static class CreateHeldTransaction
{
    public sealed record Request(
        Guid CashShiftId,
        Guid CashierId,
        Guid? CashierEmployeeId,
        Guid? CustomerId,
        Guid WarehouseId,
        Guid PosCounterId,
        Guid? PosTerminalId,
        string? CustomerName,
        string? CustomerPhone,
        string? CouponCode,
        string? Notes,
        List<PosLineInput> Lines);

    public sealed record Response(Guid Id, string ReceiptNumber);

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

            var shift = await _context.CashShifts
                .FirstOrDefaultAsync(s => s.Id == req.CashShiftId && !s.IsDeleted, ct);
            if (shift == null)
                return Result<Response>.Failure(Error.NotFound("Cash shift not found"));
            if (shift.Status != "Open")
                return Result<Response>.Failure(Error.Conflict("Cash shift is not open"));

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == req.WarehouseId && !w.IsDeleted, ct);
            if (warehouse == null)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found"));

            if (req.Lines == null || req.Lines.Count == 0)
                return Result<Response>.Failure(Error.BadRequest("At least one line item is required"));

            var now = DateTime.Now;
            var transactionId = Guid.NewGuid();
            var random4 = Random.Shared.Next(1000, 9999);
            var receiptNumber = $"POS-{warehouse.Code}-{now:yyyyMMddHHmmss}-{random4}";

            // Calculate line totals (no stock deduction, no payment)
            decimal subTotal = 0;
            decimal totalDiscount = 0;
            var lines = new List<PosTransactionLines>();

            foreach (var lineInput in req.Lines)
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == lineInput.ProductId, ct);
                if (product == null)
                    return Result<Response>.Failure(Error.NotFound($"Product {lineInput.ProductId} not found"));

                var lineDiscountAmount = lineInput.DiscountAmount;
                if (lineInput.DiscountPercent > 0 && lineDiscountAmount == 0)
                {
                    lineDiscountAmount = lineInput.UnitPrice * lineInput.Quantity * lineInput.DiscountPercent / 100m;
                }

                var lineTotal = (lineInput.UnitPrice * lineInput.Quantity) - lineDiscountAmount;

                lines.Add(new PosTransactionLines
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transactionId,
                    ProductId = lineInput.ProductId,
                    VariantId = lineInput.VariantId,
                    BatchId = lineInput.BatchId,
                    ProductName = product.Name,
                    Sku = product.Sku,
                    Quantity = lineInput.Quantity,
                    UnitPrice = lineInput.UnitPrice,
                    DiscountPercent = lineInput.DiscountPercent,
                    DiscountAmount = lineDiscountAmount,
                    TaxAmount = 0,
                    LineTotal = lineTotal,
                    CreatedAt = now,
                    CreatedBy = req.CashierId,
                    IsDeleted = false
                });

                subTotal += lineTotal;
                totalDiscount += lineDiscountAmount;
            }

            var transaction = new PosTransactions
            {
                Id = transactionId,
                ReceiptNumber = receiptNumber,
                CashShiftId = req.CashShiftId,
                PosCounterId = req.PosCounterId,
                PosTerminalId = req.PosTerminalId,
                CashierId = req.CashierId,
                CashierEmployeeId = req.CashierEmployeeId,
                CustomerId = req.CustomerId,
                WarehouseId = req.WarehouseId,
                SaleDate = now,
                SaleType = "REGULAR",
                SubTotal = subTotal,
                DiscountAmount = totalDiscount,
                TotalTaxAmount = 0,
                RoundOffAmount = 0,
                GrandTotal = subTotal,
                PaidAmount = 0,
                ChangeAmount = 0,
                TotalItemQuantity = req.Lines.Sum(l => l.Quantity),
                CouponCode = req.CouponCode,
                CustomerName = req.CustomerName,
                CustomerPhone = req.CustomerPhone,
                Status = "Held",
                Notes = req.Notes,
                CreatedAt = now,
                CreatedBy = req.CashierId
            };

            foreach (var line in lines)
                transaction.PosTransactionLines.Add(line);

            _context.PosTransactions.Add(transaction);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(transaction.Id, transaction.ReceiptNumber));
        }
    }
}
