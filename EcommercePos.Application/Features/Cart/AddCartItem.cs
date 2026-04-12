using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class AddCartItem
{
    public sealed record Request
    {
        public Guid CartId { get; init; }
        public Guid ProductId { get; init; }
        public decimal Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }

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
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .Where(c => c.Id == command.Request.CartId && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (cart == null)
                return Result<Response>.Failure(Error.NotFound("Cart not found"));

            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == command.Request.ProductId && !i.IsDeleted);
            var totalPrice = command.Request.Quantity * command.Request.UnitPrice;

            if (existingItem != null)
            {
                existingItem.Quantity += command.Request.Quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
                existingItem.UpdatedAt = DateTime.Now;
            }
            else
            {
                var item = new CartItems
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = command.Request.ProductId,
                    Quantity = command.Request.Quantity,
                    UnitPrice = command.Request.UnitPrice,
                    TotalPrice = totalPrice,
                    AddedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };
                cart.CartItems.Add(item);
            }

            cart.SubTotal = cart.CartItems.Where(i => !i.IsDeleted).Sum(i => i.Quantity * i.UnitPrice);
            cart.Total = cart.SubTotal - cart.DiscountAmount;
            cart.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = existingItem?.Id ?? cart.CartItems.Last().Id,
                Quantity = existingItem?.Quantity ?? command.Request.Quantity,
                TotalPrice = existingItem?.TotalPrice ?? totalPrice
            });
        }
    }
}
