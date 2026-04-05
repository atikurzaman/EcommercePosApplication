using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Product.Queries;

public static class GetProductById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ProductCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public string? Barcode { get; init; }
        public string? ShortDescription { get; init; }
        public string? Description { get; init; }
        public string ProductType { get; init; } = string.Empty;
        public decimal CostPrice { get; init; }
        public decimal SalePrice { get; init; }
        public decimal? OriginalPrice { get; init; }
        public bool IsTaxInclusive { get; init; }
        public decimal? WeightKg { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsActive { get; init; }
        public Guid CategoryId { get; init; }
        public Guid? BrandId { get; init; }
        public Guid? UnitId { get; init; }
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
            var item = await _context.Products
                .Where(p => p.Id == query.Id && !p.IsDeleted)
                .AsNoTracking()
                .ProjectToType<Response>()
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Product with id '{query.Id}' was not found."));
            }

            return Result<Response>.Success(item);
        }
    }
}
