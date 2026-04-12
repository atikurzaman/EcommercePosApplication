using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetAttributeTypes
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, string Name, string Slug, string UiType,
        bool AffectsPrice, bool AffectsSku, bool AffectsImage, bool AffectsStock,
        bool IsFilterable, int SortOrder, int OptionCount);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.AttributeTypes
                .AsNoTracking()
                .Where(a => !a.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(a => a.Name.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(a => a.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new Response(
                    a.Id, a.Name, a.Slug, a.UiType,
                    a.AffectsPrice, a.AffectsSku, a.AffectsImage, a.AffectsStock,
                    a.IsFilterable, a.SortOrder,
                    a.AttributeOptions.Count(o => !o.IsDeleted && o.IsActive)))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetAttributeTypeById
{
    public sealed record Query(Guid Id);

    public sealed record AttributeOptionInfo(
        Guid Id, string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed record Response(
        Guid Id, string Name, string Slug, string UiType,
        bool AffectsPrice, bool AffectsSku, bool AffectsImage, bool AffectsStock,
        bool IsFilterable, int SortOrder, List<AttributeOptionInfo> Options);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.AttributeTypes
                .AsNoTracking()
                .Where(a => a.Id == query.Id && !a.IsDeleted)
                .Select(a => new Response(
                    a.Id, a.Name, a.Slug, a.UiType,
                    a.AffectsPrice, a.AffectsSku, a.AffectsImage, a.AffectsStock,
                    a.IsFilterable, a.SortOrder,
                    a.AttributeOptions
                        .Where(o => !o.IsDeleted)
                        .OrderBy(o => o.SortOrder)
                        .Select(o => new AttributeOptionInfo(
                            o.Id, o.Value, o.DisplayValue, o.ColorId, o.SortOrder, o.IsActive))
                        .ToList()))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Attribute type not found."));

            return Result<Response>.Success(entity);
        }
    }
}

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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

public static class DeleteAttributeType
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.AttributeTypes
                .FirstOrDefaultAsync(a => a.Id == command.Id && !a.IsDeleted, ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("Attribute type not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
