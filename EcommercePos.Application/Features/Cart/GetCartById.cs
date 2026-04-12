using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class GetCartById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid? CustomerId { get; init; }
        public string? SessionId { get; init; }
        public decimal SubTotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal Total { get; init; }
        public string? CouponCode { get; init; }
        public List<CartItemResponse> Items { get; init; } = new();
    }

    public sealed record CartItemResponse
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; }
        public string? Sku { get; init; }
        public string? ImageUrl { get; init; }
        public decimal Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal TotalPrice { get; init; }
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.ProductImages)
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (cart == null)
                return Result<Response>.Failure(Error.NotFound("Cart not found"));

            var response = new Response
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                SessionId = cart.SessionId,
                SubTotal = cart.SubTotal,
                DiscountAmount = cart.DiscountAmount,
                Total = cart.Total,
                CouponCode = cart.CouponCode,
                Items = cart.CartItems.Select(i => new CartItemResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Sku = i.Product.Sku,
                    ImageUrl = i.Product.ProductImages.FirstOrDefault() != null ? i.Product.ProductImages.FirstOrDefault().ImageUrl : null,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

            return Result<Response>.Success(response);
        }
    }
}
