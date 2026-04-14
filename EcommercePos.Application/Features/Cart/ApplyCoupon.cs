using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class ApplyCoupon
{
    public sealed record Command(Guid CartId, string CouponCode);

    public sealed record Response(Guid CartId, string CouponCode, decimal DiscountAmount, decimal SubTotal, decimal Total);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CartId).NotEmpty();
            RuleFor(x => x.CouponCode).NotEmpty();
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

            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.Code == command.CouponCode && !d.IsDeleted && d.IsActive, ct);

            if (discount is null)
                return Result<Response>.Failure(Error.NotFound($"Coupon '{command.CouponCode}' not found or inactive."));

            var subtotal = cart.CartItems.Sum(i => i.TotalPrice);
            var code = discount.DiscountTypeCode?.ToLowerInvariant();
            var isPercent = code is not null && (code.Contains("percent") || code.Contains("pct"));

            var discountAmount = isPercent
                ? subtotal * (discount.DiscountValue / 100m)
                : discount.DiscountValue;

            if (discount.MaximumDiscountAmount.HasValue)
                discountAmount = Math.Min(discountAmount, discount.MaximumDiscountAmount.Value);

            cart.AppliedDiscountId = discount.Id;
            cart.CouponCode = command.CouponCode;
            cart.SubTotal = subtotal;
            cart.DiscountAmount = discountAmount;
            cart.Total = subtotal - discountAmount;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                cart.Id, command.CouponCode, discountAmount, cart.SubTotal, cart.Total));
        }
    }
}
