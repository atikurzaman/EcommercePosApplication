using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
