using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product.Queries;

public static class GetProducts
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null, Guid? CategoryId = null, Guid? BrandId = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ProductCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public string? Barcode { get; init; }
        public string? ShortDescription { get; init; }
        public decimal CostPrice { get; init; }
        public decimal SalePrice { get; init; }
        public decimal? OriginalPrice { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsActive { get; init; }
        public Guid CategoryId { get; init; }
        public Guid? BrandId { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, string? Search, Guid? CategoryId, Guid? BrandId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Products
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(p => p.Name.Contains(query.Search) || p.ProductCode.Contains(query.Search) || p.Sku.Contains(query.Search));
            }

            if (query.CategoryId.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            if (query.BrandId.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.BrandId == query.BrandId.Value);
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .ProjectToType<Response>()
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
