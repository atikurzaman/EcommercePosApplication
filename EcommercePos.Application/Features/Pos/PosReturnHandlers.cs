using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosReturns ──────────────────────────────────────────────────────────────
public static class GetPosReturns
{
    public sealed record Request(
        int PageIndex = 0,
        int PageSize = 10,
        Guid? WarehouseId = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null);

    public sealed record Response(
        Guid Id,
        string ReturnNo,
        DateTime ReturnDate,
        Guid WarehouseId,
        string WarehouseName,
        Guid? CustomerId,
        string? CustomerName,
        decimal TotalAmount,
        string? Notes,
        int ItemCount);

    public sealed record Query(
        int PageIndex,
        int PageSize,
        Guid? WarehouseId,
        DateTime? DateFrom,
        DateTime? DateTo);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.PosTransactionReturns
                .Where(r => !r.IsDeleted)
                .AsNoTracking();

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(r => r.WarehouseId == query.WarehouseId.Value);

            if (query.DateFrom.HasValue)
                dbQuery = dbQuery.Where(r => r.ReturnDate >= query.DateFrom.Value);

            if (query.DateTo.HasValue)
                dbQuery = dbQuery.Where(r => r.ReturnDate <= query.DateTo.Value);

            var totalCount = await dbQuery.CountAsync(ct);

            var items = await dbQuery
                .Include(r => r.Warehouse)
                .Include(r => r.Customer)
                .Include(r => r.PosTransactionReturnLines)
                .OrderByDescending(r => r.ReturnDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new Response(
                    r.Id,
                    r.ReturnNo,
                    r.ReturnDate,
                    r.WarehouseId,
                    r.Warehouse.Name,
                    r.CustomerId,
                    r.Customer != null ? (r.Customer.CompanyName ?? r.Customer.CustomerCode) : null,
                    r.TotalAmount,
                    r.Notes,
                    r.PosTransactionReturnLines.Count(l => !l.IsDeleted)))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}

// ── GetPosReturnById ───────────────────────────────────────────────────────────
public static class GetPosReturnById
{
    public sealed record ReturnLineInfo(
        Guid Id,
        Guid ProductId,
        Guid? VariantId,
        Guid? BatchId,
        string ProductName,
        string? Sku,
        decimal Quantity,
        decimal UnitPrice,
        decimal LineTotal);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ReturnNo { get; init; } = string.Empty;
        public DateTime ReturnDate { get; init; }
        public Guid WarehouseId { get; init; }
        public string WarehouseName { get; init; } = string.Empty;
        public Guid? CustomerId { get; init; }
        public string? CustomerName { get; init; }
        public Guid? SaleId { get; init; }
        public string? SaleReceiptNumber { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Notes { get; init; }
        public Guid? CreatedByUserId { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<ReturnLineInfo> Lines { get; init; } = new();
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var ret = await _context.PosTransactionReturns
                .Include(r => r.Warehouse)
                .Include(r => r.Customer)
                .Include(r => r.Sale)
                .Include(r => r.PosTransactionReturnLines)
                    .ThenInclude(l => l.Product)
                .Where(r => r.Id == query.Id && !r.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (ret == null)
                return Result<Response>.Failure(Error.NotFound("Return not found"));

            var response = new Response
            {
                Id = ret.Id,
                ReturnNo = ret.ReturnNo,
                ReturnDate = ret.ReturnDate,
                WarehouseId = ret.WarehouseId,
                WarehouseName = ret.Warehouse?.Name ?? string.Empty,
                CustomerId = ret.CustomerId,
                CustomerName = ret.Customer != null ? (ret.Customer.CompanyName ?? ret.Customer.CustomerCode) : null,
                SaleId = ret.SaleId,
                SaleReceiptNumber = ret.Sale?.ReceiptNumber,
                TotalAmount = ret.TotalAmount,
                Notes = ret.Notes,
                CreatedByUserId = ret.CreatedByUserId,
                CreatedAt = ret.CreatedAt,
                Lines = ret.PosTransactionReturnLines
                    .Where(l => !l.IsDeleted)
                    .Select(l => new ReturnLineInfo(
                        l.Id,
                        l.ProductId,
                        l.VariantId,
                        l.BatchId,
                        l.Product?.Name ?? string.Empty,
                        l.Product?.Sku,
                        l.Quantity,
                        l.UnitPrice,
                        l.LineTotal))
                    .ToList()
            };

            return Result<Response>.Success(response);
        }
    }
}

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
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
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
