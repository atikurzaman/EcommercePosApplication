using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetProductPriceHistory
{
    public sealed record Request(Guid ProductId, int PageIndex = 0, int PageSize = 20);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ChangedByUserId { get; init; }
        public string? ChangedByName { get; init; }
        public decimal OldCostPrice { get; init; }
        public decimal OldSalePrice { get; init; }
        public decimal NewCostPrice { get; init; }
        public decimal NewSalePrice { get; init; }
        public DateTime EffectiveFrom { get; init; }
        public DateTime? EffectiveTo { get; init; }
        public string? Reason { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(Guid ProductId, int PageIndex, int PageSize);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.ProductPriceHistories
                .Where(x => x.ProductId == query.ProductId && !x.IsDeleted);

            var totalCount = await q.CountAsync(ct);

            var items = await q
                .OrderByDescending(x => x.CreatedAt)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Join(
                    _context.Users,
                    h => h.ChangedByUserId,
                    u => u.Id,
                    (h, u) => new Response
                    {
                        Id = h.Id,
                        ChangedByUserId = h.ChangedByUserId,
                        ChangedByName = u.UserName,
                        OldCostPrice = h.OldCostPrice,
                        OldSalePrice = h.OldSalePrice,
                        NewCostPrice = h.NewCostPrice,
                        NewSalePrice = h.NewSalePrice,
                        EffectiveFrom = h.EffectiveFrom,
                        EffectiveTo = h.EffectiveTo,
                        Reason = h.Reason,
                        CreatedAt = h.CreatedAt
                    })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
