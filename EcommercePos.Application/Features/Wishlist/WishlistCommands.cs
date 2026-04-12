using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Wishlist;

public static class GetWishlists
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public int ItemCount { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(Guid CustomerId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var wishlists = await _context.Wishlists
                .Where(w => w.CustomerId == query.CustomerId && !w.IsDeleted)
                .Include(w => w.WishlistItems.Where(i => !i.IsDeleted))
                .AsNoTracking()
                .ToListAsync(ct);

            var result = wishlists.Select(w => new Response
            {
                Id = w.Id,
                CustomerId = w.CustomerId ?? Guid.Empty,
                ItemCount = w.WishlistItems.Count,
                CreatedAt = w.CreatedAt
            }).ToList();

            return Result<List<Response>>.Success(result);
        }
    }
}

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
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
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

public static class AddWishlistItem
{
    public sealed record Request(Guid WishlistId, Guid ProductId);

    public sealed record Response
    {
        public Guid Id { get; init; }
    }

    public sealed record Command(Request Request);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var existing = await _context.WishlistItems
                .FirstOrDefaultAsync(i => i.WishlistId == command.Request.WishlistId && i.ProductId == command.Request.ProductId && !i.IsDeleted, ct);

            if (existing != null)
                return Result<Response>.Failure(Error.Conflict("Product already in wishlist"));

            var item = new WishlistItems
            {
                Id = Guid.NewGuid(),
                WishlistId = command.Request.WishlistId,
                ProductId = command.Request.ProductId,
                AddedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response { Id = item.Id });
        }
    }
}

public static class RemoveWishlistItem
{
    public sealed record Command(Guid ItemId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.WishlistItems
                .Where(i => i.Id == command.ItemId && !i.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result.Failure(Error.NotFound("Wishlist item not found"));

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
