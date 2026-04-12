using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetShiftSummary ────────────────────────────────────────────────────────────
public static class GetShiftSummary
{
    public sealed record Query(Guid ShiftId);

    public sealed record TransactionInfo(
        Guid Id, string ReceiptNumber, DateTime SaleDate,
        string SaleType, decimal GrandTotal, string Status);

    public sealed record CashDrawerEventInfo(
        Guid Id, string EventType, decimal Amount,
        string? Notes, DateTime OccurredAt,
        Guid PerformedBy, string? PerformedByName);

    public sealed record PaymentBreakdown(string MethodCode, decimal TotalAmount, int Count);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        Guid PosCounterId, string CounterName,
        Guid? OpenedByUserId, string? OpenedByName,
        Guid? ClosedByUserId, string? ClosedByName,
        string Status, DateTime OpeningDateTime, DateTime? ClosingDateTime,
        decimal OpeningCash, decimal? ClosingCash,
        decimal? ExpectedCash, decimal? CashVariance,
        decimal TotalSalesAmount, int TotalTransactions,
        string? Notes,
        List<TransactionInfo> Transactions,
        List<CashDrawerEventInfo> CashDrawerEvents,
        List<PaymentBreakdown> PaymentBreakdowns);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var shift = await _context.CashShifts
                .AsNoTracking()
                .Where(s => s.Id == query.ShiftId && !s.IsDeleted)
                .Select(s => new
                {
                    s.Id, s.WarehouseId,
                    WarehouseName = s.Warehouse.Name,
                    s.PosCounterId,
                    CounterName = s.PosCounter.CounterName,
                    s.OpenedByUserId,
                    OpenedByName = s.OpenedByUser != null ? (s.OpenedByUser.FirstName + " " + s.OpenedByUser.LastName) : null,
                    s.ClosedByUserId,
                    ClosedByName = s.ClosedByUser != null ? (s.ClosedByUser.FirstName + " " + s.ClosedByUser.LastName) : null,
                    s.Status, s.OpeningDateTime, s.ClosingDateTime,
                    s.OpeningCash, s.ClosingCash,
                    s.ExpectedCash, s.CashVariance,
                    s.TotalSalesAmount, s.TotalTransactions,
                    s.Notes
                })
                .FirstOrDefaultAsync(ct);

            if (shift == null)
                return Result<Response>.Failure(Error.NotFound("Shift not found."));

            var transactions = await _context.PosTransactions
                .AsNoTracking()
                .Where(t => t.CashShiftId == query.ShiftId && !t.IsDeleted)
                .OrderByDescending(t => t.SaleDate)
                .Select(t => new TransactionInfo(
                    t.Id, t.ReceiptNumber, t.SaleDate,
                    t.SaleType, t.GrandTotal, t.Status))
                .ToListAsync(ct);

            var events = await _context.CashDrawerEvents
                .AsNoTracking()
                .Where(e => e.CashShiftId == query.ShiftId && !e.IsDeleted)
                .OrderBy(e => e.OccurredAt)
                .Select(e => new CashDrawerEventInfo(
                    e.Id, e.EventType, e.Amount,
                    e.Notes, e.OccurredAt,
                    e.PerformedBy,
                    e.PerformedByNavigation != null ? (e.PerformedByNavigation.FirstName + " " + e.PerformedByNavigation.LastName) : null))
                .ToListAsync(ct);

            var paymentBreakdowns = await _context.PosPaymentTenders
                .AsNoTracking()
                .Where(p => p.Transaction.CashShiftId == query.ShiftId && !p.IsDeleted)
                .GroupBy(p => p.MethodCode)
                .Select(g => new PaymentBreakdown(g.Key, g.Sum(p => p.Amount), g.Count()))
                .ToListAsync(ct);

            return Result<Response>.Success(new Response(
                shift.Id, shift.WarehouseId, shift.WarehouseName,
                shift.PosCounterId, shift.CounterName,
                shift.OpenedByUserId, shift.OpenedByName,
                shift.ClosedByUserId, shift.ClosedByName,
                shift.Status, shift.OpeningDateTime, shift.ClosingDateTime,
                shift.OpeningCash, shift.ClosingCash,
                shift.ExpectedCash, shift.CashVariance,
                shift.TotalSalesAmount, shift.TotalTransactions,
                shift.Notes, transactions, events, paymentBreakdowns));
        }
    }
}
