using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetCollectionById
{
    public sealed record Query(Guid Id);

    public sealed record CollectionProductInfo(
        Guid Id, Guid ProductId, string ProductName, string? ProductCode,
        string? ImageUrl, decimal SalePrice, int DisplayOrder);

    public sealed record Response(
        Guid Id, string Name, string Slug, string? Description, string? ImageUrl,
        int DisplayOrder, bool IsActive, bool ShowInHomePage,
        List<CollectionProductInfo> Products);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.ProductCollections
                .AsNoTracking()
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound($"Collection with id '{query.Id}' was not found."));

            var collectionItems = await _context.ProductCollectionItems
                .AsNoTracking()
                .Where(i => i.ProductCollectionId == query.Id && !i.IsDeleted)
                .Select(i => new { i.Id, i.ProductId, i.DisplayOrder })
                .ToListAsync(ct);

            var productIds = collectionItems.Select(i => i.ProductId).ToList();

            var products = await _context.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
                .Select(p => new { p.Id, p.Name, p.ProductCode, p.SalePrice })
                .ToListAsync(ct);

            var productMap = products.ToDictionary(p => p.Id);

            var productInfos = collectionItems
                .Where(i => productMap.ContainsKey(i.ProductId))
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new CollectionProductInfo(
                    i.Id, i.ProductId,
                    productMap[i.ProductId].Name,
                    productMap[i.ProductId].ProductCode,
                    null,
                    productMap[i.ProductId].SalePrice,
                    i.DisplayOrder))
                .ToList();

            return Result<Response>.Success(new Response(
                entity.Id, entity.Name, entity.Slug, entity.Description, entity.ImageUrl,
                entity.DisplayOrder, entity.IsActive, entity.ShowInHomePage, productInfos));
        }
    }
}
