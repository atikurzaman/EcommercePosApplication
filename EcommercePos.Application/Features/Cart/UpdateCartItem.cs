using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class UpdateCartItem
{
    public sealed record Command(Guid ItemId, decimal Quantity);

    public sealed record Response(Guid Id, Guid CartId, decimal Quantity, decimal UnitPrice, decimal TotalPrice);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.CartItems
                .Where(i => i.Id == command.ItemId && !i.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Cart item '{command.ItemId}' was not found."));

            item.Quantity = command.Quantity;
            item.TotalPrice = command.Quantity * item.UnitPrice;
            item.UpdatedAt = DateTime.UtcNow;

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == item.CartId && !c.IsDeleted, ct);

            if (cart is not null)
            {
                var subtotal = await _context.CartItems
                    .Where(i => i.CartId == cart.Id && !i.IsDeleted && i.Id != item.Id)
                    .SumAsync(i => (decimal?)i.TotalPrice, ct) ?? 0m;

                cart.SubTotal = subtotal + item.TotalPrice;
                cart.Total = cart.SubTotal - cart.DiscountAmount;
                cart.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                item.Id, item.CartId, item.Quantity, item.UnitPrice, item.TotalPrice));
        }
    }
}
