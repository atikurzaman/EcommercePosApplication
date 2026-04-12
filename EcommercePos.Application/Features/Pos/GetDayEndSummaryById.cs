using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetDayEndSummaryById ───────────────────────────────────────────────────────
public static class GetDayEndSummaryById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, DateOnly SummaryDate, Guid WarehouseId, string WarehouseName,
        Guid? CashShiftId,
        int TotalSalesCount, decimal TotalSalesAmount,
        decimal TotalCashSales, decimal TotalCardSales, decimal TotalMobileSales,
        decimal TotalReturnAmount, decimal TotalDiscount, decimal TotalTaxCollected,
        decimal OpeningCash, decimal CashInHand, decimal CashOut,
        decimal ExpectedCash, decimal Variance,
        int TotalItemsSold, int TotalTransactions,
        int NewCustomers, int ReturningCustomers,
        decimal LoyaltyPointsIssued, decimal LoyaltyPointsRedeemed,
        string Status, string? Notes, DateTime? ClosedAt,
        Guid? ClosedByUserId, string? ClosedByName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.DayEndSummaries
                .AsNoTracking()
                .Where(d => d.Id == query.Id && !d.IsDeleted)
                .Select(d => new Response(
                    d.Id, d.SummaryDate, d.WarehouseId, d.Warehouse.Name,
                    d.CashShiftId,
                    d.TotalSalesCount, d.TotalSalesAmount,
                    d.TotalCashSales, d.TotalCardSales, d.TotalMobileSales,
                    d.TotalReturnAmount, d.TotalDiscount, d.TotalTaxCollected,
                    d.OpeningCash, d.CashInHand, d.CashOut,
                    d.ExpectedCash, d.Variance,
                    d.TotalItemsSold, d.TotalTransactions,
                    d.NewCustomers, d.ReturningCustomers,
                    d.LoyaltyPointsIssued, d.LoyaltyPointsRedeemed,
                    d.Status, d.Notes, d.ClosedAt,
                    d.ClosedByUserId,
                    d.ClosedByUser != null ? (d.ClosedByUser.FirstName + " " + d.ClosedByUser.LastName) : null))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Day-end summary not found."));

            return Result<Response>.Success(entity);
        }
    }
}
