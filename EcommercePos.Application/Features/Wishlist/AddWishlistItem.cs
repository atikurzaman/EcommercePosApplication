using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Wishlist;

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
