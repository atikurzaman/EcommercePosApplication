using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product;

public static class GetProductStats
{
    public sealed record Query();
    public sealed record Response(
        int TotalProducts,
        int ActiveProducts,
        int FeaturedProducts,
        int LowStockProducts);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var totalProducts = await _context.Products.Where(p => !p.IsDeleted).CountAsync(ct);
            var activeProducts = await _context.Products.Where(p => !p.IsDeleted && p.IsActive).CountAsync(ct);
            var featuredProducts = await _context.Products.Where(p => !p.IsDeleted && p.IsFeatured).CountAsync(ct);
            var lowStockProducts = await _context.Products
                .Include(p => p.StockItems)
                .Where(p => !p.IsDeleted && p.StockItems.Any(s => s.QuantityOnHand <= p.ReorderLevel))
                .CountAsync(ct);
            return Result<Response>.Success(new Response(totalProducts, activeProducts, featuredProducts, lowStockProducts));
        }
    }
}