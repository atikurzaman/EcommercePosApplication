using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetHomePageCollections
{
    public sealed record CollectionProductInfo(
        Guid ProductId, string ProductName, string? ProductCode,
        string? ImageUrl, decimal SalePrice, int DisplayOrder);

    public sealed record Response(
        Guid Id, string Name, string Slug, string? Description, string? ImageUrl,
        int DisplayOrder, List<CollectionProductInfo> Products);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(CancellationToken ct)
        {
            var collections = await _context.ProductCollections
                .AsNoTracking()
                .Where(c => !c.IsDeleted && c.IsActive && c.ShowInHomePage)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new { c.Id, c.Name, c.Slug, c.Description, c.ImageUrl, c.DisplayOrder })
                .ToListAsync(ct);

            if (collections.Count == 0)
                return Result<List<Response>>.Success([]);

            var collectionIds = collections.Select(c => c.Id).ToList();

            var collectionItems = await _context.ProductCollectionItems
                .AsNoTracking()
                .Where(i => collectionIds.Contains(i.ProductCollectionId) && !i.IsDeleted)
                .Select(i => new { i.ProductCollectionId, i.ProductId, i.DisplayOrder })
                .ToListAsync(ct);

            var productIds = collectionItems.Select(i => i.ProductId).Distinct().ToList();

            var products = await _context.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id) && !p.IsDeleted && p.IsActive)
                .Select(p => new { p.Id, p.Name, p.ProductCode, p.SalePrice })
                .ToListAsync(ct);

            var productMap = products.ToDictionary(p => p.Id);

            var result = collections.Select(c => new Response(
                c.Id, c.Name, c.Slug, c.Description, c.ImageUrl, c.DisplayOrder,
                collectionItems
                    .Where(i => i.ProductCollectionId == c.Id && productMap.ContainsKey(i.ProductId))
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new CollectionProductInfo(
                        i.ProductId,
                        productMap[i.ProductId].Name,
                        productMap[i.ProductId].ProductCode,
                        null,
                        productMap[i.ProductId].SalePrice,
                        i.DisplayOrder))
                    .ToList()
            )).ToList();

            return Result<List<Response>>.Success(result);
        }
    }
}
