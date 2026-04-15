using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ShippingMethod;

public static class GetShippingMethods
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, string Name, string? Description, string? CarrierName, decimal BaseCost,
        decimal CostPerKg, int EstimatedDaysMin, int EstimatedDaysMax, bool IsActive, bool IsFreeShipping);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.ShippingMethods
                .Where(s => !s.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(s => s.Name.Contains(query.Search));

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(s => s.DisplayOrder)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(s => new Response(
                    s.Id, s.Name, s.Description, s.CarrierName, s.BaseCost,
                    s.CostPerKg, s.EstimatedDaysMin, s.EstimatedDaysMax, s.IsActive, s.IsFreeShipping))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
