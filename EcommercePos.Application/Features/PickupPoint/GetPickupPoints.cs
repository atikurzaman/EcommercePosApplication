using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.PickupPoint;

public static class GetPickupPoints
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, Guid? WarehouseId, string Name, string AddressLine1, string City,
        string? PostalCode, string Phone, decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.PickupPoints
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(p => p.Name.Contains(query.Search) || p.City.Contains(query.Search));

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(p => p.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new Response(
                    p.Id, p.WarehouseId, p.Name, p.AddressLine1, p.City,
                    p.PostalCode, p.Phone, p.Latitude, p.Longitude,
                    p.OpeningTime, p.ClosingTime, p.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
