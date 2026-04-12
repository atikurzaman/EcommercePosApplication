using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class RemoveCartItem
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
            var item = await _context.CartItems
                .Where(i => i.Id == command.ItemId && !i.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result.Failure(Error.NotFound("Cart item not found"));

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;

            var cart = await _context.Carts.FindAsync(new object[] { item.CartId }, ct);
            if (cart != null)
            {
                var items = await _context.CartItems.Where(i => i.CartId == cart.Id && !i.IsDeleted).ToListAsync(ct);
                cart.SubTotal = items.Sum(i => i.Quantity * i.UnitPrice);
                cart.Total = cart.SubTotal - cart.DiscountAmount;
                cart.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
