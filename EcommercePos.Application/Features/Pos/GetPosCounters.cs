using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosCounters ─────────────────────────────────────────────────────────────
public static class GetPosCounters
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, Guid? WarehouseId = null, string? Search = null);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        string CounterCode, string CounterName, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.PosCounters
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (request.WarehouseId.HasValue)
                query = query.Where(c => c.WarehouseId == request.WarehouseId.Value);
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.CounterName.Contains(request.Search) || c.CounterCode.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.CounterCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(
                    c.Id, c.WarehouseId, c.Warehouse.Name,
                    c.CounterCode, c.CounterName, c.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
