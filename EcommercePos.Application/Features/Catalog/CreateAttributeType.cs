using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class CreateAttributeType
{
    public sealed record Request(
        string Name, string? Slug, string UiType,
        bool AffectsPrice, bool AffectsSku, bool AffectsImage, bool AffectsStock,
        bool IsFilterable, int SortOrder);

    public sealed record Response(Guid Id, string Name, string Slug);

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-").Replace("--", "-");

            var entity = new AttributeTypes
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                UiType = request.UiType,
                AffectsPrice = request.AffectsPrice,
                AffectsSku = request.AffectsSku,
                AffectsImage = request.AffectsImage,
                AffectsStock = request.AffectsStock,
                IsFilterable = request.IsFilterable,
                SortOrder = request.SortOrder,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.AttributeTypes.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Name, entity.Slug));
        }
    }
}
