using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

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
                .Include(t => t.PosPaymentTenders)
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
