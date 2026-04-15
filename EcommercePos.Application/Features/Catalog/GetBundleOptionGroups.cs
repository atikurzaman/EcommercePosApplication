using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetBundleOptionGroups
{
    public sealed record Query(Guid BundleProductId);

    public sealed record BundleOptionItemInfo(
        Guid Id, Guid VariantId, string VariantName, string ProductName,
        decimal PriceAdjustment, bool IsDefault, int SortOrder);

    public sealed record Response(
        Guid Id, string GroupName, bool IsRequired,
        int MinSelections, int MaxSelections, int QuantityPerSelection, int SortOrder,
        List<BundleOptionItemInfo> Items);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var groups = await _context.BundleOptionGroups
                .AsNoTracking()
                .Where(g => g.BundleProductId == query.BundleProductId && !g.IsDeleted)
                .OrderBy(g => g.SortOrder)
                .Select(g => new Response(
                    g.Id,
                    g.GroupName,
                    g.IsRequired,
                    g.MinSelections,
                    g.MaxSelections,
                    g.QuantityPerSelection,
                    g.SortOrder,
                    g.BundleOptionItems
                        .Where(i => !i.IsDeleted)
                        .OrderBy(i => i.SortOrder)
                        .Join(_context.ProductVariants.Where(v => !v.IsDeleted),
                            i => i.VariantId, v => v.Id,
                            (i, v) => new { Item = i, Variant = v })
                        .Join(_context.Products.Where(p => !p.IsDeleted),
                            iv => iv.Variant.ProductId, p => p.Id,
                            (iv, p) => new BundleOptionItemInfo(
                                iv.Item.Id,
                                iv.Item.VariantId,
                                iv.Variant.Name,
                                p.Name,
                                iv.Item.PriceAdjustment,
                                iv.Item.IsDefault,
                                iv.Item.SortOrder))
                        .ToList()))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(groups);
        }
    }
}
