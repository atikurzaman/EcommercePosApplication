using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetProductVariants
{
    public sealed record Request(Guid ProductId);

    public sealed record VariantAttributeInfo(
        Guid AttributeTypeId, string AttributeTypeName, Guid OptionId, string OptionValue);

    public sealed record Response(
        Guid Id, string Name, string? Sku, string? Barcode,
        decimal CostPrice, decimal PriceModifier, decimal? OverridePrice,
        decimal? WeightKg, bool IsDefault, bool IsActive, int SortOrder,
        string? ImageUrl, List<VariantAttributeInfo> Attributes);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Request request, CancellationToken ct)
        {
            var variants = await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.ProductId == request.ProductId && !v.IsDeleted)
                .OrderBy(v => v.SortOrder)
                .Select(v => new Response(
                    v.Id, v.Name, v.Sku, v.Barcode,
                    v.CostPrice, v.PriceModifier, v.OverridePrice,
                    v.WeightKg, v.IsDefault, v.IsActive, v.SortOrder,
                    v.ImageUrl,
                    v.VariantAttributeOptions
                        .Where(vao => !vao.IsDeleted)
                        .Select(vao => new VariantAttributeInfo(
                            vao.Option.AttributeTypeId,
                            vao.Option.AttributeType.Name,
                            vao.OptionId,
                            vao.Option.Value))
                        .ToList()))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(variants);
        }
    }
}
