using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class AddCartItem
{
    public sealed record Command(Guid CartId, Guid ProductId, decimal Quantity, decimal UnitPrice);

    public sealed record Response(Guid Id, Guid CartId, Guid ProductId, decimal Quantity, decimal UnitPrice, decimal TotalPrice);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CartId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems.Where(i => !i.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == command.CartId && !c.IsDeleted, ct);

            if (cart is null)
                return Result<Response>.Failure(Error.NotFound($"Cart '{command.CartId}' was not found."));

            var productExists = await _context.Products
                .AnyAsync(p => p.Id == command.ProductId && !p.IsDeleted, ct);

            if (!productExists)
                return Result<Response>.Failure(Error.NotFound($"Product '{command.ProductId}' was not found."));

            var item = new CartItems
            {
                Id = Guid.NewGuid(),
                CartId = command.CartId,
                ProductId = command.ProductId,
                Quantity = command.Quantity,
                UnitPrice = command.UnitPrice,
                TotalPrice = command.Quantity * command.UnitPrice,
                AddedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.CartItems.Add(item);

            cart.SubTotal = cart.CartItems.Sum(i => i.TotalPrice) + item.TotalPrice;
            cart.Total = cart.SubTotal - cart.DiscountAmount;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                item.Id, item.CartId, item.ProductId, item.Quantity, item.UnitPrice, item.TotalPrice));
        }
    }
}
