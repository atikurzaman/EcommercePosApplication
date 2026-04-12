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
