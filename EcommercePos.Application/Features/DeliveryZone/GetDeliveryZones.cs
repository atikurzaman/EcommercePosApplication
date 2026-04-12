using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.DeliveryZone;

public static class GetDeliveryZones
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, string Name, string? Description, bool IsActive,
        decimal BaseDeliveryCost, decimal? FreeDeliveryThreshold,
        int? MinDeliveryDays, int? MaxDeliveryDays);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.DeliveryZones
                .Where(z => !z.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(z => z.Name.Contains(query.Search));

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(z => z.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(z => new Response(
                    z.Id, z.Name, z.Description, z.IsActive,
                    z.BaseDeliveryCost, z.FreeDeliveryThreshold,
                    z.MinDeliveryDays, z.MaxDeliveryDays))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
