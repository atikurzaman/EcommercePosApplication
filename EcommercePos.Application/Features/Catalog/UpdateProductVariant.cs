using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class UpdateProductVariant
{
    public sealed record Request(
        string Name, string? Sku, string? Barcode,
        decimal CostPrice, decimal PriceModifier, decimal? OverridePrice,
        decimal? WeightKg, bool IsDefault, bool IsActive, int SortOrder,
        string? ImageUrl, List<Guid>? AttributeOptionIds);

    public sealed record Command(
        Guid Id, string Name, string? Sku, string? Barcode,
        decimal CostPrice, decimal PriceModifier, decimal? OverridePrice,
        decimal? WeightKg, bool IsDefault, bool IsActive, int SortOrder,
        string? ImageUrl, List<Guid>? AttributeOptionIds);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
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

        public async Task<Result<GetProductVariants.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ProductVariants
                .Include(v => v.VariantAttributeOptions)
                .FirstOrDefaultAsync(v => v.Id == command.Id && !v.IsDeleted, ct);

            if (entity == null)
                return Result<GetProductVariants.Response>.Failure(Error.NotFound("Product variant not found."));

            // If IsDefault, unset other defaults for the same product
            if (command.IsDefault && !entity.IsDefault)
            {
                var existingDefaults = await _context.ProductVariants
                    .Where(v => v.ProductId == entity.ProductId && v.IsDefault && !v.IsDeleted && v.Id != entity.Id)
                    .ToListAsync(ct);

                foreach (var d in existingDefaults)
                    d.IsDefault = false;
            }

            entity.Name = command.Name;
            entity.Sku = command.Sku;
            entity.Barcode = command.Barcode;
            entity.CostPrice = command.CostPrice;
            entity.PriceModifier = command.PriceModifier;
            entity.OverridePrice = command.OverridePrice;
            entity.WeightKg = command.WeightKg;
            entity.IsDefault = command.IsDefault;
            entity.IsActive = command.IsActive;
            entity.SortOrder = command.SortOrder;
            entity.ImageUrl = command.ImageUrl;
            entity.UpdatedAt = DateTime.UtcNow;

            // Replace VariantAttributeOptions
            var existingOptions = entity.VariantAttributeOptions.Where(o => !o.IsDeleted).ToList();
            foreach (var opt in existingOptions)
                opt.IsDeleted = true;

            if (command.AttributeOptionIds is { Count: > 0 })
            {
                foreach (var optionId in command.AttributeOptionIds)
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

            // Reload with attributes for response
            var response = await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.Id == entity.Id)
                .Select(v => new GetProductVariants.Response(
                    v.Id, v.Name, v.Sku, v.Barcode,
                    v.CostPrice, v.PriceModifier, v.OverridePrice,
                    v.WeightKg, v.IsDefault, v.IsActive, v.SortOrder,
                    v.ImageUrl,
                    v.VariantAttributeOptions
                        .Where(vao => !vao.IsDeleted)
                        .Select(vao => new GetProductVariants.VariantAttributeInfo(
                            vao.Option.AttributeTypeId,
                            vao.Option.AttributeType.Name,
                            vao.OptionId,
                            vao.Option.Value))
                        .ToList()))
                .FirstAsync(ct);

            return Result<GetProductVariants.Response>.Success(response);
        }
    }
}
