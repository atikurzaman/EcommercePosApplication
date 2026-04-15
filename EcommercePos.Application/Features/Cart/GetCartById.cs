using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class GetCartById
{
    public sealed record Query(Guid Id);

    public sealed record ItemResponse(
        Guid Id, Guid ProductId, Guid? VariantId, Guid? BatchId,
        decimal Quantity, decimal UnitPrice, decimal TotalPrice, DateTime AddedAt);

    public sealed record Response(
        Guid Id, Guid? CustomerId, Guid? UserId, string? SessionId,
        decimal SubTotal, decimal DiscountAmount, decimal Total,
        Guid? AppliedDiscountId, string? CouponCode,
        DateTime CreatedAt, IReadOnlyList<ItemResponse> Items);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Carts
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .AsNoTracking()
                .Select(c => new Response(
                    c.Id, c.CustomerId, c.UserId, c.SessionId,
                    c.CartItems.Where(i => !i.IsDeleted).Sum(i => (decimal?)i.TotalPrice) ?? 0m,
                    c.DiscountAmount,
                    (c.CartItems.Where(i => !i.IsDeleted).Sum(i => (decimal?)i.TotalPrice) ?? 0m) - c.DiscountAmount,
                    c.AppliedDiscountId, c.CouponCode, c.CreatedAt,
                    c.CartItems
                        .Where(i => !i.IsDeleted)
                        .Select(i => new ItemResponse(
                            i.Id, i.ProductId, i.VariantId, i.BatchId,
                            i.Quantity, i.UnitPrice, i.TotalPrice, i.AddedAt))
                        .ToList()))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Cart '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
