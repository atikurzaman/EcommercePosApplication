using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class ApplyCoupon
{
    public sealed record Request(Guid CartId, string CouponCode);

    public sealed record Response
    {
        public string CouponCode { get; init; } = string.Empty;
        public decimal DiscountAmount { get; init; }
        public decimal Total { get; init; }
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
            var discount = await _context.Discounts
                .Where(d => d.Code == command.Request.CouponCode && d.IsActive && !d.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (discount == null)
                return Result<Response>.Failure(Error.NotFound("Invalid coupon code"));

            var cart = await _context.Carts
                .Where(c => c.Id == command.Request.CartId && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (cart == null)
                return Result<Response>.Failure(Error.NotFound("Cart not found"));

            decimal discountAmount = 0;
            if (discount.DiscountTypeCode == "PERCENTAGE")
            {
                discountAmount = cart.SubTotal * discount.DiscountValue / 100;
            }
            else
            {
                discountAmount = discount.DiscountValue;
            }

            cart.AppliedDiscountId = discount.Id;
            cart.CouponCode = command.Request.CouponCode;
            cart.DiscountAmount += discountAmount;
            cart.Total = cart.SubTotal - cart.DiscountAmount;
            cart.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                CouponCode = command.Request.CouponCode,
                DiscountAmount = discountAmount,
                Total = cart.Total
            });
        }
    }
}
