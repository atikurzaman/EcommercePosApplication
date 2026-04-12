using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product;

public static class GetProducts
{
    public sealed record Query(
        int PageIndex = 0, int PageSize = 10, string? Search = null,
        Guid? CategoryId = null, Guid? BrandId = null);

    public sealed record Response(
        Guid Id, string ProductCode, string Name, string? Sku, string? Barcode,
        string? ShortDescription, decimal CostPrice, decimal SalePrice, decimal? OriginalPrice,
        bool IsFeatured, bool IsActive, string ProductType, Guid CategoryId, Guid? BrandId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Products.Where(p => !p.IsDeleted).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(p =>
                    p.Name.Contains(query.Search) ||
                    p.ProductCode.Contains(query.Search) ||
                    (p.Sku != null && p.Sku.Contains(query.Search)));

            if (query.CategoryId.HasValue)
                dbQuery = dbQuery.Where(p => p.CategoryId == query.CategoryId.Value);

            if (query.BrandId.HasValue)
                dbQuery = dbQuery.Where(p => p.BrandId == query.BrandId.Value);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(p => p.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new Response(
                    p.Id, p.ProductCode, p.Name, p.Sku, p.Barcode,
                    p.ShortDescription, p.CostPrice, p.SalePrice, p.OriginalPrice,
                    p.IsFeatured, p.IsActive, p.ProductType, p.CategoryId, p.BrandId))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
