using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetDayEndSummaries ─────────────────────────────────────────────────────────
public static class GetDayEndSummaries
{
    public sealed record Request(
        int PageIndex = 0, int PageSize = 10,
        Guid? WarehouseId = null, DateOnly? DateFrom = null, DateOnly? DateTo = null);

    public sealed record Response(
        Guid Id, DateOnly SummaryDate, Guid WarehouseId, string WarehouseName,
        int TotalSalesCount, decimal TotalSalesAmount,
        decimal TotalCashSales, decimal TotalCardSales, decimal TotalMobileSales,
        decimal TotalReturnAmount, decimal TotalDiscount, decimal TotalTaxCollected,
        decimal OpeningCash, decimal CashInHand, decimal CashOut,
        decimal ExpectedCash, decimal Variance,
        int TotalItemsSold, int TotalTransactions,
        string Status, DateTime? ClosedAt);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.DayEndSummaries
                .AsNoTracking()
                .Where(d => !d.IsDeleted);

            if (request.WarehouseId.HasValue)
                query = query.Where(d => d.WarehouseId == request.WarehouseId.Value);
            if (request.DateFrom.HasValue)
                query = query.Where(d => d.SummaryDate >= request.DateFrom.Value);
            if (request.DateTo.HasValue)
                query = query.Where(d => d.SummaryDate <= request.DateTo.Value);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(d => d.SummaryDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(d => new Response(
                    d.Id, d.SummaryDate, d.WarehouseId, d.Warehouse.Name,
                    d.TotalSalesCount, d.TotalSalesAmount,
                    d.TotalCashSales, d.TotalCardSales, d.TotalMobileSales,
                    d.TotalReturnAmount, d.TotalDiscount, d.TotalTaxCollected,
                    d.OpeningCash, d.CashInHand, d.CashOut,
                    d.ExpectedCash, d.Variance,
                    d.TotalItemsSold, d.TotalTransactions,
                    d.Status, d.ClosedAt))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
