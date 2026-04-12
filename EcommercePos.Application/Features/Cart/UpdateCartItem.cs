using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class UpdateCartItem
{
    public sealed record Request(Guid ItemId, decimal Quantity);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public decimal Quantity { get; init; }
        public decimal TotalPrice { get; init; }
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
            var item = await _context.CartItems
                .Where(i => i.Id == command.Request.ItemId && !i.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result<Response>.Failure(Error.NotFound("Cart item not found"));

            item.Quantity = command.Request.Quantity;
            item.TotalPrice = item.Quantity * item.UnitPrice;
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

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice
            });
        }
    }
}
