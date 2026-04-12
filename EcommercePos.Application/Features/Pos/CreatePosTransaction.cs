using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

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
