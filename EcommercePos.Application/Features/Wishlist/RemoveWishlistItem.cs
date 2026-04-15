using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Wishlist;

public static class RemoveWishlistItem
{
    public sealed record Command(Guid ItemId);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
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
