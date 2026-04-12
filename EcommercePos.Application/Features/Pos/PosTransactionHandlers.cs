using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosTransactions ─────────────────────────────────────────────────────────
public static class GetPosTransactions
{
    public sealed record Request(
        int PageIndex = 0,
        int PageSize = 10,
        string? Search = null,
        Guid? CashShiftId = null,
        Guid? CashierId = null,
        Guid? WarehouseId = null,
        string? Status = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null);

    public sealed record Response(
        Guid Id,
        string ReceiptNumber,
        DateTime SaleDate,
        string Status,
        string? CustomerName,
        string? CustomerPhone,
        decimal SubTotal,
        decimal DiscountAmount,
        decimal TotalTaxAmount,
        decimal GrandTotal,
        decimal PaidAmount,
        decimal ChangeAmount,
        int ItemCount,
        string CashierName,
        string WarehouseName);

    public sealed record Query(
        int PageIndex,
        int PageSize,
        string? Search,
        Guid? CashShiftId,
        Guid? CashierId,
        Guid? WarehouseId,
        string? Status,
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
            var dbQuery = _context.PosTransactions
                .Where(t => !t.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(t =>
                    t.ReceiptNumber.Contains(query.Search) ||
                    (t.CustomerName != null && t.CustomerName.Contains(query.Search)) ||
                    (t.CustomerPhone != null && t.CustomerPhone.Contains(query.Search)));
            }

            if (query.CashShiftId.HasValue)
                dbQuery = dbQuery.Where(t => t.CashShiftId == query.CashShiftId.Value);

            if (query.CashierId.HasValue)
                dbQuery = dbQuery.Where(t => t.CashierId == query.CashierId.Value);

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(t => t.WarehouseId == query.WarehouseId.Value);

            if (!string.IsNullOrWhiteSpace(query.Status))
                dbQuery = dbQuery.Where(t => t.Status == query.Status);

            if (query.DateFrom.HasValue)
                dbQuery = dbQuery.Where(t => t.SaleDate >= query.DateFrom.Value);

            if (query.DateTo.HasValue)
                dbQuery = dbQuery.Where(t => t.SaleDate <= query.DateTo.Value);

            var totalCount = await dbQuery.CountAsync(ct);

            var items = await dbQuery
                .Include(t => t.Cashier)
                .Include(t => t.Warehouse)
                .Include(t => t.PosTransactionLines)
                .OrderByDescending(t => t.SaleDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new Response(
                    t.Id,
                    t.ReceiptNumber,
                    t.SaleDate,
                    t.Status,
                    t.CustomerName,
                    t.CustomerPhone,
                    t.SubTotal,
                    t.DiscountAmount,
                    t.TotalTaxAmount,
                    t.GrandTotal,
                    t.PaidAmount,
                    t.ChangeAmount,
                    t.PosTransactionLines.Count(l => !l.IsDeleted),
                    t.Cashier.UserName ?? string.Empty,
                    t.Warehouse.Name))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}

// ── GetPosTransactionById ──────────────────────────────────────────────────────
public static class GetPosTransactionById
{
    public sealed record TransactionLineInfo(
        Guid Id,
        Guid ProductId,
        Guid? VariantId,
        string ProductName,
        string? Sku,
        decimal Quantity,
        decimal UnitPrice,
        decimal DiscountPercent,
        decimal DiscountAmount,
        decimal TaxAmount,
        decimal LineTotal);

    public sealed record PaymentTenderInfo(
        Guid Id,
        string MethodCode,
        decimal Amount,
        string? TransactionNo,
        string? CardLast4,
        DateTime PaymentDate);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ReceiptNumber { get; init; } = string.Empty;
        public DateTime SaleDate { get; init; }
        public string SaleType { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid CashShiftId { get; init; }
        public Guid PosCounterId { get; init; }
        public Guid? PosTerminalId { get; init; }
        public Guid CashierId { get; init; }
        public string CashierName { get; init; } = string.Empty;
        public Guid? CustomerId { get; init; }
        public string? CustomerName { get; init; }
        public string? CustomerPhone { get; init; }
        public Guid WarehouseId { get; init; }
        public string WarehouseName { get; init; } = string.Empty;
        public string? CouponCode { get; init; }
        public decimal SubTotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal TotalTaxAmount { get; init; }
        public decimal RoundOffAmount { get; init; }
        public decimal GrandTotal { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal ChangeAmount { get; init; }
        public decimal TotalItemQuantity { get; init; }
        public string? VoidReason { get; init; }
        public Guid? VoidedBy { get; init; }
        public DateTime? VoidedAt { get; init; }
        public string? Notes { get; init; }
        public List<TransactionLineInfo> Lines { get; init; } = new();
        public List<PaymentTenderInfo> Payments { get; init; } = new();
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
            var transaction = await _context.PosTransactions
                .Include(t => t.Cashier)
                .Include(t => t.Warehouse)
                .Include(t => t.PosTransactionLines)
                .Include(t => t.PosPaymentTenders)
                .Where(t => t.Id == query.Id && !t.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (transaction == null)
                return Result<Response>.Failure(Error.NotFound("Transaction not found"));

            var response = new Response
            {
                Id = transaction.Id,
                ReceiptNumber = transaction.ReceiptNumber,
                SaleDate = transaction.SaleDate,
                SaleType = transaction.SaleType,
                Status = transaction.Status,
                CashShiftId = transaction.CashShiftId,
                PosCounterId = transaction.PosCounterId,
                PosTerminalId = transaction.PosTerminalId,
                CashierId = transaction.CashierId,
                CashierName = transaction.Cashier?.UserName ?? string.Empty,
                CustomerId = transaction.CustomerId,
                CustomerName = transaction.CustomerName,
                CustomerPhone = transaction.CustomerPhone,
                WarehouseId = transaction.WarehouseId,
                WarehouseName = transaction.Warehouse?.Name ?? string.Empty,
                CouponCode = transaction.CouponCode,
                SubTotal = transaction.SubTotal,
                DiscountAmount = transaction.DiscountAmount,
                TotalTaxAmount = transaction.TotalTaxAmount,
                RoundOffAmount = transaction.RoundOffAmount,
                GrandTotal = transaction.GrandTotal,
                PaidAmount = transaction.PaidAmount,
                ChangeAmount = transaction.ChangeAmount,
                TotalItemQuantity = transaction.TotalItemQuantity,
                VoidReason = transaction.VoidReason,
                VoidedBy = transaction.VoidedBy,
                VoidedAt = transaction.VoidedAt,
                Notes = transaction.Notes,
                Lines = transaction.PosTransactionLines
                    .Where(l => !l.IsDeleted)
                    .Select(l => new TransactionLineInfo(
                        l.Id,
                        l.ProductId,
                        l.VariantId,
                        l.ProductName,
                        l.Sku,
                        l.Quantity,
                        l.UnitPrice,
                        l.DiscountPercent,
                        l.DiscountAmount,
                        l.TaxAmount,
                        l.LineTotal))
                    .ToList(),
                Payments = transaction.PosPaymentTenders
                    .Where(p => !p.IsDeleted)
                    .Select(p => new PaymentTenderInfo(
                        p.Id,
                        p.MethodCode,
                        p.Amount,
                        p.TransactionNo,
                        p.CardLast4,
                        p.PaymentDate))
                    .ToList()
            };

            return Result<Response>.Success(response);
        }
    }
}

// ── Shared input records ───────────────────────────────────────────────────────
public sealed record PosLineInput(
    Guid ProductId,
    Guid? VariantId,
    Guid? BatchId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    decimal DiscountAmount);

public sealed record PaymentTenderInput(
    string MethodCode,
    decimal Amount,
    string? TransactionNo,
    string? CardLast4);

// ── CreatePosTransaction ───────────────────────────────────────────────────────
public static class CreatePosTransaction
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
        List<PosLineInput> Lines,
        List<PaymentTenderInput> Payments);

    public sealed record Response(
        Guid Id,
        string ReceiptNumber,
        decimal GrandTotal,
        decimal PaidAmount,
        decimal ChangeAmount);

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

            // 1. Validate CashShift is Open
            var shift = await _context.CashShifts
                .FirstOrDefaultAsync(s => s.Id == req.CashShiftId && !s.IsDeleted, ct);
            if (shift == null)
                return Result<Response>.Failure(Error.NotFound("Cash shift not found"));
            if (shift.Status != "Open")
                return Result<Response>.Failure(Error.Conflict("Cash shift is not open"));

            // Validate POS counter exists
            var counter = await _context.PosCounters.FindAsync(new object[] { req.PosCounterId }, ct);
            if (counter == null)
                return Result<Response>.Failure(Error.NotFound("POS counter not found"));

            // Look up warehouse for receipt code
            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == req.WarehouseId && !w.IsDeleted, ct);
            if (warehouse == null)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found"));

            if (req.Lines == null || req.Lines.Count == 0)
                return Result<Response>.Failure(Error.BadRequest("At least one line item is required"));

            if (req.Payments == null || req.Payments.Count == 0)
                return Result<Response>.Failure(Error.BadRequest("At least one payment is required"));

            // 2. Generate ReceiptNumber: POS-{WarehouseCode}-{yyyyMMddHHmmss}-{random4digits}
            var random4 = Random.Shared.Next(1000, 9999);
            var receiptNumber = $"POS-{warehouse.Code}-{DateTime.Now:yyyyMMddHHmmss}-{random4}";

            // 3. Calculate line totals
            var now = DateTime.Now;
            var transactionId = Guid.NewGuid();
            decimal subTotal = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;
            var lines = new List<PosTransactionLines>();

            foreach (var lineInput in req.Lines)
            {
                var lineDiscountAmount = lineInput.DiscountAmount;
                if (lineInput.DiscountPercent > 0 && lineDiscountAmount == 0)
                {
                    lineDiscountAmount = lineInput.UnitPrice * lineInput.Quantity * lineInput.DiscountPercent / 100m;
                }

                var lineTotal = (lineInput.UnitPrice * lineInput.Quantity) - lineDiscountAmount;

                // Look up product name/sku
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == lineInput.ProductId, ct);
                if (product == null)
                    return Result<Response>.Failure(Error.NotFound($"Product {lineInput.ProductId} not found"));

                var line = new PosTransactionLines
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
                    TaxAmount = 0, // Tax calculated at line level if tax rates are applied
                    LineTotal = lineTotal,
                    CreatedAt = now,
                    CreatedBy = req.CashierId,
                    IsDeleted = false
                };

                lines.Add(line);
                subTotal += lineTotal;
                totalDiscount += lineDiscountAmount;
                totalTax += line.TaxAmount;
            }

            var grandTotal = subTotal + totalTax;

            // 4. Validate PaidAmount >= GrandTotal
            var paidAmount = req.Payments.Sum(p => p.Amount);
            if (paidAmount < grandTotal)
                return Result<Response>.Failure(Error.BadRequest(
                    $"Paid amount ({paidAmount:F2}) is less than grand total ({grandTotal:F2})"));

            // 5. Calculate ChangeAmount
            var changeAmount = paidAmount - grandTotal;

            // Create the transaction entity
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
                TotalTaxAmount = totalTax,
                RoundOffAmount = 0,
                GrandTotal = grandTotal,
                PaidAmount = paidAmount,
                ChangeAmount = changeAmount,
                TotalItemQuantity = req.Lines.Sum(l => l.Quantity),
                CouponCode = req.CouponCode,
                CustomerName = req.CustomerName,
                CustomerPhone = req.CustomerPhone,
                Status = "Completed",
                Notes = req.Notes,
                CreatedAt = now,
                CreatedBy = req.CashierId
            };

            // Add lines
            foreach (var line in lines)
            {
                transaction.PosTransactionLines.Add(line);
            }

            // Add payment tenders
            foreach (var payment in req.Payments)
            {
                transaction.PosPaymentTenders.Add(new PosPaymentTenders
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transactionId,
                    MethodCode = payment.MethodCode,
                    Amount = payment.Amount,
                    TransactionNo = payment.TransactionNo,
                    CardLast4 = payment.CardLast4,
                    PaymentDate = now,
                    CreatedAt = now,
                    CreatedBy = req.CashierId
                });
            }

            // 6. Deduct stock and create StockMovements for each line
            foreach (var line in lines)
            {
                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s =>
                        s.ProductId == line.ProductId &&
                        s.WarehouseId == req.WarehouseId &&
                        !s.IsDeleted, ct);

                if (stockItem != null)
                {
                    var qtyBefore = stockItem.QuantityOnHand;
                    stockItem.QuantityOnHand -= line.Quantity;
                    stockItem.LastUpdatedAt = now;

                    _context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        BatchId = line.BatchId,
                        StockItemId = stockItem.Id,
                        FromWarehouseId = req.WarehouseId,
                        MovementTypeCode = "SALE",
                        QuantityIn = 0,
                        QuantityOut = line.Quantity,
                        BalanceAfter = stockItem.QuantityOnHand,
                        UnitCost = line.UnitPrice,
                        ReferenceType = "POS_TRANSACTION",
                        ReferenceId = transactionId,
                        ReferenceNumber = receiptNumber,
                        Notes = $"POS sale line for {line.ProductName}",
                        OccurredAt = now,
                        CreatedAt = now,
                        CreatedBy = req.CashierId
                    });
                }
            }

            // 8. Record CashDrawerEvent for SALE
            _context.CashDrawerEvents.Add(new CashDrawerEvents
            {
                Id = Guid.NewGuid(),
                CashShiftId = req.CashShiftId,
                PerformedBy = req.CashierId,
                TransactionId = transactionId,
                EventType = "SALE",
                Amount = grandTotal,
                Notes = $"Sale {receiptNumber}",
                OccurredAt = now,
                CreatedAt = now,
                CreatedBy = req.CashierId
            });

            // 9. Update CashShift totals
            shift.TotalSalesAmount += grandTotal;
            shift.TotalTransactions += 1;
            shift.UpdatedAt = now;
            shift.UpdatedBy = req.CashierId;

            _context.PosTransactions.Add(transaction);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                transaction.Id,
                transaction.ReceiptNumber,
                transaction.GrandTotal,
                transaction.PaidAmount,
                transaction.ChangeAmount));
        }
    }
}

// ── VoidPosTransaction ─────────────────────────────────────────────────────────
public static class VoidPosTransaction
{
    public sealed record Command(Guid TransactionId, Guid VoidedBy, string VoidReason);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var transaction = await _context.PosTransactions
                .Include(t => t.PosTransactionLines)
                .Where(t => t.Id == command.TransactionId && !t.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (transaction == null)
                return Result.Failure(Error.NotFound("Transaction not found"));

            if (transaction.Status == "Voided")
                return Result.Failure(Error.Conflict("Transaction is already voided"));

            var now = DateTime.Now;
            transaction.Status = "Voided";
            transaction.VoidReason = command.VoidReason;
            transaction.VoidedBy = command.VoidedBy;
            transaction.VoidedAt = now;
            transaction.UpdatedAt = now;
            transaction.UpdatedBy = command.VoidedBy;

            // Restore stock quantities and create reverse StockMovements
            foreach (var line in transaction.PosTransactionLines.Where(l => !l.IsDeleted))
            {
                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s =>
                        s.ProductId == line.ProductId &&
                        s.WarehouseId == transaction.WarehouseId &&
                        !s.IsDeleted, ct);

                if (stockItem != null)
                {
                    stockItem.QuantityOnHand += line.Quantity;
                    stockItem.LastUpdatedAt = now;

                    _context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        BatchId = line.BatchId,
                        StockItemId = stockItem.Id,
                        ToWarehouseId = transaction.WarehouseId,
                        MovementTypeCode = "VOID",
                        QuantityIn = line.Quantity,
                        QuantityOut = 0,
                        BalanceAfter = stockItem.QuantityOnHand,
                        UnitCost = line.UnitPrice,
                        ReferenceType = "POS_VOID",
                        ReferenceId = transaction.Id,
                        ReferenceNumber = transaction.ReceiptNumber,
                        Notes = $"Void reversal for {line.ProductName}",
                        OccurredAt = now,
                        CreatedAt = now,
                        CreatedBy = command.VoidedBy
                    });
                }
            }

            // Update CashShift totals
            var shift = await _context.CashShifts.FindAsync(new object[] { transaction.CashShiftId }, ct);
            if (shift != null)
            {
                shift.TotalSalesAmount -= transaction.GrandTotal;
                shift.TotalTransactions -= 1;
                shift.UpdatedAt = now;
                shift.UpdatedBy = command.VoidedBy;
            }

            // Record CashDrawerEvent for VOID
            _context.CashDrawerEvents.Add(new CashDrawerEvents
            {
                Id = Guid.NewGuid(),
                CashShiftId = transaction.CashShiftId,
                PerformedBy = command.VoidedBy,
                TransactionId = transaction.Id,
                EventType = "VOID",
                Amount = -transaction.GrandTotal,
                Notes = $"Voided {transaction.ReceiptNumber}: {command.VoidReason}",
                OccurredAt = now,
                CreatedAt = now,
                CreatedBy = command.VoidedBy
            });

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}

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
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
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

// ── ResumeHeldTransaction ──────────────────────────────────────────────────────
public static class ResumeHeldTransaction
{
    public sealed record Command(Guid TransactionId, List<PaymentTenderInput> Payments);

    public sealed record Response(
        Guid Id,
        string ReceiptNumber,
        decimal GrandTotal,
        decimal PaidAmount,
        decimal ChangeAmount);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var transaction = await _context.PosTransactions
                .Include(t => t.PosTransactionLines)
                .Where(t => t.Id == command.TransactionId && !t.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (transaction == null)
                return Result<Response>.Failure(Error.NotFound("Transaction not found"));

            if (transaction.Status != "Held")
                return Result<Response>.Failure(Error.Conflict("Transaction is not in Held status"));

            if (command.Payments == null || command.Payments.Count == 0)
                return Result<Response>.Failure(Error.BadRequest("At least one payment is required"));

            var now = DateTime.Now;
            var paidAmount = command.Payments.Sum(p => p.Amount);
            var grandTotal = transaction.GrandTotal;

            if (paidAmount < grandTotal)
                return Result<Response>.Failure(Error.BadRequest(
                    $"Paid amount ({paidAmount:F2}) is less than grand total ({grandTotal:F2})"));

            var changeAmount = paidAmount - grandTotal;

            // Update transaction
            transaction.PaidAmount = paidAmount;
            transaction.ChangeAmount = changeAmount;
            transaction.Status = "Completed";
            transaction.SaleDate = now;
            transaction.UpdatedAt = now;
            transaction.UpdatedBy = transaction.CashierId;

            // Add payment tenders
            foreach (var payment in command.Payments)
            {
                transaction.PosPaymentTenders.Add(new PosPaymentTenders
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transaction.Id,
                    MethodCode = payment.MethodCode,
                    Amount = payment.Amount,
                    TransactionNo = payment.TransactionNo,
                    CardLast4 = payment.CardLast4,
                    PaymentDate = now,
                    CreatedAt = now,
                    CreatedBy = transaction.CashierId
                });
            }

            // Deduct stock and create StockMovements
            foreach (var line in transaction.PosTransactionLines.Where(l => !l.IsDeleted))
            {
                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s =>
                        s.ProductId == line.ProductId &&
                        s.WarehouseId == transaction.WarehouseId &&
                        !s.IsDeleted, ct);

                if (stockItem != null)
                {
                    stockItem.QuantityOnHand -= line.Quantity;
                    stockItem.LastUpdatedAt = now;

                    _context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        BatchId = line.BatchId,
                        StockItemId = stockItem.Id,
                        FromWarehouseId = transaction.WarehouseId,
                        MovementTypeCode = "SALE",
                        QuantityIn = 0,
                        QuantityOut = line.Quantity,
                        BalanceAfter = stockItem.QuantityOnHand,
                        UnitCost = line.UnitPrice,
                        ReferenceType = "POS_TRANSACTION",
                        ReferenceId = transaction.Id,
                        ReferenceNumber = transaction.ReceiptNumber,
                        Notes = $"POS sale (resumed) line for {line.ProductName}",
                        OccurredAt = now,
                        CreatedAt = now,
                        CreatedBy = transaction.CashierId
                    });
                }
            }

            // Record CashDrawerEvent for SALE
            _context.CashDrawerEvents.Add(new CashDrawerEvents
            {
                Id = Guid.NewGuid(),
                CashShiftId = transaction.CashShiftId,
                PerformedBy = transaction.CashierId,
                TransactionId = transaction.Id,
                EventType = "SALE",
                Amount = grandTotal,
                Notes = $"Sale (resumed) {transaction.ReceiptNumber}",
                OccurredAt = now,
                CreatedAt = now,
                CreatedBy = transaction.CashierId
            });

            // Update CashShift totals
            var shift = await _context.CashShifts.FindAsync(new object[] { transaction.CashShiftId }, ct);
            if (shift != null)
            {
                shift.TotalSalesAmount += grandTotal;
                shift.TotalTransactions += 1;
                shift.UpdatedAt = now;
                shift.UpdatedBy = transaction.CashierId;
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                transaction.Id,
                transaction.ReceiptNumber,
                transaction.GrandTotal,
                transaction.PaidAmount,
                transaction.ChangeAmount));
        }
    }
}
