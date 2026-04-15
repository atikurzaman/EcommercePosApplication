using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class GetCarts
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, Guid? CustomerId = null, Guid? UserId = null);

    public sealed record Response(
        Guid Id, Guid? CustomerId, Guid? UserId, string? SessionId,
        decimal SubTotal, decimal DiscountAmount, decimal Total,
        string? CouponCode, int ItemCount, DateTime CreatedAt);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Carts.Where(c => !c.IsDeleted).AsNoTracking();

            if (query.CustomerId.HasValue)
                dbQuery = dbQuery.Where(c => c.CustomerId == query.CustomerId);

            if (query.UserId.HasValue)
                dbQuery = dbQuery.Where(c => c.UserId == query.UserId);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderByDescending(c => c.CreatedAt)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new Response(
                    c.Id, c.CustomerId, c.UserId, c.SessionId,
                    c.SubTotal, c.DiscountAmount, c.Total,
                    c.CouponCode,
                    c.CartItems.Count(i => !i.IsDeleted),
                    c.CreatedAt))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
