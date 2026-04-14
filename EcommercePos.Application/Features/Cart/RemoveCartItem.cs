using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class RemoveCartItem
{
    public sealed record Command(Guid ItemId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.CartItems
                .Where(i => i.Id == command.ItemId && !i.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result.Failure(Error.NotFound($"Cart item '{command.ItemId}' was not found."));

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == item.CartId && !c.IsDeleted, ct);

            if (cart is not null)
            {
                var subtotal = await _context.CartItems
                    .Where(i => i.CartId == cart.Id && !i.IsDeleted && i.Id != item.Id)
                    .SumAsync(i => (decimal?)i.TotalPrice, ct) ?? 0m;

                cart.SubTotal = subtotal;
                cart.Total = cart.SubTotal - cart.DiscountAmount;
                cart.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
