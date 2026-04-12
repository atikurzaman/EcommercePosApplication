using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

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
