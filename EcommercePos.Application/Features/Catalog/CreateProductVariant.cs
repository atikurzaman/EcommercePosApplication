using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class CreateProductVariant
{
    public sealed record Request(
        Guid ProductId, string Name, string? Sku, string? Barcode,
        decimal CostPrice, decimal PriceModifier, decimal? OverridePrice,
        decimal? WeightKg, bool IsDefault, bool IsActive, int SortOrder,
        string? ImageUrl, List<Guid>? AttributeOptionIds);

    public sealed record Response(Guid Id, string Name);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Sku).MaximumLength(100);
            RuleFor(x => x.Barcode).MaximumLength(100);
            RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var productExists = await _context.Products
                .AnyAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);

            if (!productExists)
                return Result<Response>.Failure(Error.NotFound("Product not found."));

            // If IsDefault, unset other defaults for the same product
            if (request.IsDefault)
            {
                var existingDefaults = await _context.ProductVariants
                    .Where(v => v.ProductId == request.ProductId && v.IsDefault && !v.IsDeleted)
                    .ToListAsync(ct);

                foreach (var d in existingDefaults)
                    d.IsDefault = false;
            }

            var entity = new ProductVariants
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Name = request.Name,
                Sku = request.Sku,
                Barcode = request.Barcode,
                CostPrice = request.CostPrice,
                PriceModifier = request.PriceModifier,
                OverridePrice = request.OverridePrice,
                WeightKg = request.WeightKg,
                IsDefault = request.IsDefault,
                IsActive = request.IsActive,
                SortOrder = request.SortOrder,
                ImageUrl = request.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ProductVariants.Add(entity);

            if (request.AttributeOptionIds is { Count: > 0 })
            {
                foreach (var optionId in request.AttributeOptionIds)
                {
                    _context.VariantAttributeOptions.Add(new VariantAttributeOptions
                    {
                        Id = Guid.NewGuid(),
                        VariantId = entity.Id,
                        OptionId = optionId,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Name));
        }
    }
}
