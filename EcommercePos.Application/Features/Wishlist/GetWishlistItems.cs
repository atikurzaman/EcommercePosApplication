using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Wishlist;

public static class GetWishlistItems
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public string? ImageUrl { get; init; }
        public decimal Price { get; init; }
        public DateTime AddedAt { get; init; }
    }

    public sealed record Query(Guid WishlistId);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.WishlistItems
                .Include(i => i.Product)
                .Where(i => i.WishlistId == query.WishlistId && !i.IsDeleted)
                .AsNoTracking()
                .Select(i => new Response
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Sku = i.Product.Sku,
                    ImageUrl = i.Product.ProductImages.FirstOrDefault() != null ? i.Product.ProductImages.FirstOrDefault().ImageUrl : null,
                    Price = i.Product.SalePrice,
                    AddedAt = i.AddedAt
                })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
