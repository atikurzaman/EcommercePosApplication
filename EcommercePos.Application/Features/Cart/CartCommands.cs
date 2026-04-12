using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Cart;

public static class GetCarts
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, Guid? CustomerId = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid? CustomerId { get; init; }
        public decimal SubTotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal Total { get; init; }
        public int ItemCount { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, Guid? CustomerId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Carts
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (query.CustomerId.HasValue)
            {
                dbQuery = dbQuery.Where(c => c.CustomerId == query.CustomerId.Value);
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .Include(c => c.CartItems)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new Response
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,
                    SubTotal = c.SubTotal,
                    DiscountAmount = c.DiscountAmount,
                    Total = c.Total,
                    ItemCount = c.CartItems.Count(i => !i.IsDeleted),
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}

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

public static class CreateCart
{
    public sealed record Request(Guid? CustomerId, string? SessionId);

    public sealed record Response
    {
        public Guid Id { get; init; }
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
            var cart = new Carts
            {
                Id = Guid.NewGuid(),
                CustomerId = command.Request.CustomerId,
                SessionId = command.Request.SessionId ?? Guid.NewGuid().ToString(),
                SubTotal = 0,
                DiscountAmount = 0,
                Total = 0,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response { Id = cart.Id });
        }
    }
}

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
