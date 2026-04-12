using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetCashShifts ──────────────────────────────────────────────────────────────
public static class GetCashShifts
{
    public sealed record Request(
        int PageIndex = 0, int PageSize = 10,
        Guid? WarehouseId = null, string? Status = null,
        DateTime? DateFrom = null, DateTime? DateTo = null);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        Guid PosCounterId, string CounterName,
        Guid? OpenedByUserId, string? OpenedByName,
        string Status, DateTime OpeningDateTime, DateTime? ClosingDateTime,
        decimal OpeningCash, decimal? ClosingCash,
        decimal TotalSalesAmount, int TotalTransactions);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.CashShifts
                .AsNoTracking()
                .Where(s => !s.IsDeleted);

            if (request.WarehouseId.HasValue)
                query = query.Where(s => s.WarehouseId == request.WarehouseId.Value);
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status == request.Status);
            if (request.DateFrom.HasValue)
                query = query.Where(s => s.OpeningDateTime >= request.DateFrom.Value);
            if (request.DateTo.HasValue)
                query = query.Where(s => s.OpeningDateTime <= request.DateTo.Value);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(s => s.OpeningDateTime)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new Response(
                    s.Id, s.WarehouseId, s.Warehouse.Name,
                    s.PosCounterId, s.PosCounter.CounterName,
                    s.OpenedByUserId, s.OpenedByUser != null ? (s.OpenedByUser.FirstName + " " + s.OpenedByUser.LastName) : null,
                    s.Status, s.OpeningDateTime, s.ClosingDateTime,
                    s.OpeningCash, s.ClosingCash,
                    s.TotalSalesAmount, s.TotalTransactions))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
