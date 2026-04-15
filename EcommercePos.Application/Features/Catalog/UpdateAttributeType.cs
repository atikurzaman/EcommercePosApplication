using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class UpdateAttributeType
{
    public sealed record Request(
        string Name, string? Slug, string UiType,
        bool AffectsPrice, bool AffectsSku, bool AffectsImage, bool AffectsStock,
        bool IsFilterable, int SortOrder);

    public sealed record Command(
        Guid Id, string Name, string? Slug, string UiType,
        bool AffectsPrice, bool AffectsSku, bool AffectsImage, bool AffectsStock,
        bool IsFilterable, int SortOrder);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(80);
            RuleFor(x => x.Slug).MaximumLength(100);
            RuleFor(x => x.UiType).NotEmpty().MaximumLength(20)
                .Must(v => v is "Dropdown" or "ColorSwatch" or "RadioButton" or "Checkbox")
                .WithMessage("UiType must be one of: Dropdown, ColorSwatch, RadioButton, Checkbox.");
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetAttributeTypeById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.AttributeTypes
                .FirstOrDefaultAsync(a => a.Id == command.Id && !a.IsDeleted, ct);

            if (entity == null)
                return Result<GetAttributeTypeById.Response>.Failure(Error.NotFound("Attribute type not found."));

            entity.Name = command.Name;
            entity.Slug = command.Slug ?? command.Name.ToLower().Replace(" ", "-").Replace("--", "-");
            entity.UiType = command.UiType;
            entity.AffectsPrice = command.AffectsPrice;
            entity.AffectsSku = command.AffectsSku;
            entity.AffectsImage = command.AffectsImage;
            entity.AffectsStock = command.AffectsStock;
            entity.IsFilterable = command.IsFilterable;
            entity.SortOrder = command.SortOrder;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            // Reload with options
            var options = await _context.AttributeOptions
                .AsNoTracking()
                .Where(o => o.AttributeTypeId == entity.Id && !o.IsDeleted)
                .OrderBy(o => o.SortOrder)
                .Select(o => new GetAttributeTypeById.AttributeOptionInfo(
                    o.Id, o.Value, o.DisplayValue, o.ColorId, o.SortOrder, o.IsActive))
                .ToListAsync(ct);

            return Result<GetAttributeTypeById.Response>.Success(
                new GetAttributeTypeById.Response(
                    entity.Id, entity.Name, entity.Slug, entity.UiType,
                    entity.AffectsPrice, entity.AffectsSku, entity.AffectsImage, entity.AffectsStock,
                    entity.IsFilterable, entity.SortOrder, options));
        }
    }
}
