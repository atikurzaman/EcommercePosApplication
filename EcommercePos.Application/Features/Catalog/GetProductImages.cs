using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetProductImages
{
    public sealed record Request(Guid ProductId, Guid? VariantId = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public Guid? VariantId { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public string? AltText { get; init; }
        public int SortOrder { get; init; }
        public bool IsPrimary { get; init; }
    }

    public sealed record Query(Guid ProductId, Guid? VariantId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.ProductImages
                .Where(x => x.ProductId == query.ProductId && !x.IsDeleted);

            if (query.VariantId.HasValue)
                q = q.Where(x => x.VariantId == query.VariantId.Value);

            var items = await q
                .OrderBy(x => x.SortOrder)
                .Select(x => new Response
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    ImageUrl = x.ImageUrl,
                    AltText = x.AltText,
                    SortOrder = x.SortOrder,
                    IsPrimary = x.IsPrimary
                })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
