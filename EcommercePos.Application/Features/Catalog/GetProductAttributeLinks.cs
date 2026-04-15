using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetProductAttributeLinks
{
    public sealed record Request(Guid ProductId);

    public sealed record AttributeOptionInfo(
        Guid Id, string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed record Response(
        Guid Id, Guid AttributeTypeId, string AttributeTypeName, string UiType,
        bool IsRequired, int SortOrder, List<AttributeOptionInfo> Options);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Request request, CancellationToken ct)
        {
            var items = await _context.ProductAttributeLinks
                .AsNoTracking()
                .Where(l => l.ProductId == request.ProductId && !l.IsDeleted)
                .OrderBy(l => l.SortOrder)
                .Select(l => new Response(
                    l.Id, l.AttributeTypeId, l.AttributeType.Name, l.AttributeType.UiType,
                    l.IsRequired, l.SortOrder,
                    l.AttributeType.AttributeOptions
                        .Where(o => !o.IsDeleted)
                        .OrderBy(o => o.SortOrder)
                        .Select(o => new AttributeOptionInfo(
                            o.Id, o.Value, o.DisplayValue, o.ColorId, o.SortOrder, o.IsActive))
                        .ToList()))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
