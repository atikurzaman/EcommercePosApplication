using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetBundleComponents
{
    public sealed record Query(Guid BundleProductId);

    public sealed record Response(
        Guid Id, Guid ComponentVariantId, string VariantName, string ProductName,
        decimal Quantity, bool IsSubstitutable, int SortOrder);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.BundleComponents
                .AsNoTracking()
                .Where(c => c.BundleProductId == query.BundleProductId && !c.IsDeleted)
                .OrderBy(c => c.SortOrder)
                .Join(_context.ProductVariants.Where(v => !v.IsDeleted),
                    c => c.ComponentVariantId, v => v.Id,
                    (c, v) => new { Component = c, Variant = v })
                .Join(_context.Products.Where(p => !p.IsDeleted),
                    cv => cv.Variant.ProductId, p => p.Id,
                    (cv, p) => new Response(
                        cv.Component.Id,
                        cv.Component.ComponentVariantId,
                        cv.Variant.Name,
                        p.Name,
                        cv.Component.Quantity,
                        cv.Component.IsSubstitutable,
                        cv.Component.SortOrder))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
