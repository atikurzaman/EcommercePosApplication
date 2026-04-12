using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product;

public static class GetProductById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string ProductCode, string Name, string? Sku, string? Barcode,
        string? ShortDescription, string? Description, string ProductType,
        decimal CostPrice, decimal SalePrice, decimal? OriginalPrice,
        bool IsTaxInclusive, decimal? WeightKg, bool IsFeatured, bool IsActive,
        Guid CategoryId, Guid? BrandId, Guid? UnitId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Products
                .Where(p => p.Id == query.Id && !p.IsDeleted)
                .AsNoTracking()
                .Select(p => new Response(
                    p.Id, p.ProductCode, p.Name, p.Sku, p.Barcode,
                    p.ShortDescription, p.Description, p.ProductType,
                    p.CostPrice, p.SalePrice, p.OriginalPrice,
                    p.IsTaxInclusive, p.WeightKg, p.IsFeatured, p.IsActive,
                    p.CategoryId, p.BrandId, p.UnitId))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Product '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
