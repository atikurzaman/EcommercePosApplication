using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class GetCarts
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, Guid? CustomerId = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid? CustomerId { get; init; }
        public decimal SubTotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal Total { get; init; }
        public int ItemCount { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, Guid? CustomerId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Carts
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (query.CustomerId.HasValue)
            {
                dbQuery = dbQuery.Where(c => c.CustomerId == query.CustomerId.Value);
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .Include(c => c.CartItems)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new Response
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,
                    SubTotal = c.SubTotal,
                    DiscountAmount = c.DiscountAmount,
                    Total = c.Total,
                    ItemCount = c.CartItems.Count(i => !i.IsDeleted),
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
