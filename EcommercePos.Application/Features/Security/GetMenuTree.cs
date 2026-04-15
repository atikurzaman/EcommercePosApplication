using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetMenuTree
{
    public sealed record MenuTreeItem(
        Guid Id, string MenuCode, string DisplayName, string? MenuUrl,
        string? IconClass, int DisplayOrder, byte MenuLevel,
        bool IsActive, bool IsVisible, List<MenuTreeItem> Children);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<MenuTreeItem>>> Handle(CancellationToken ct)
        {
            var allMenus = await _context.Menus
                .Where(m => !m.IsDeleted)
                .AsNoTracking()
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(ct);

            var lookup = allMenus.ToLookup(m => m.ParentMenuId);

            List<MenuTreeItem> BuildChildren(Guid? parentId)
            {
                return lookup[parentId]
                    .Select(m => new MenuTreeItem(
                        m.Id, m.MenuCode, m.DisplayName, m.MenuUrl,
                        m.IconClass, m.DisplayOrder, m.MenuLevel,
                        m.IsActive, m.IsVisible, BuildChildren(m.Id)))
                    .ToList();
            }

            var tree = BuildChildren(null);
            return Result<List<MenuTreeItem>>.Success(tree);
        }
    }
}
