using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetMenus
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(
        Guid Id, string MenuCode, string MenuName, string DisplayName, string? MenuUrl,
        string? IconClass, int DisplayOrder, byte MenuLevel, string? PermissionCode,
        Guid? ParentMenuId, bool IsActive, bool IsVisible);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Menus
                .Where(m => !m.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(m =>
                    m.MenuCode.Contains(request.Search) ||
                    m.MenuName.Contains(request.Search) ||
                    m.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(m => m.DisplayOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new Response(
                    m.Id, m.MenuCode, m.MenuName, m.DisplayName, m.MenuUrl,
                    m.IconClass, m.DisplayOrder, m.MenuLevel, m.PermissionCode,
                    m.ParentMenuId, m.IsActive, m.IsVisible))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
