using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetUserMenus
{
    public sealed record Query(Guid UserId);

    public sealed record MenuNode(
        Guid Id, Guid? ParentMenuId, string MenuCode, string MenuName, string DisplayName,
        string? MenuUrl, string? IconClass, int DisplayOrder, byte MenuLevel,
        bool IsExternalLink, bool OpenInNewTab, List<MenuNode> Children);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<MenuNode>>> Handle(Query query, CancellationToken ct)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == query.UserId, ct);
            if (!userExists)
                return Result<List<MenuNode>>.Failure(Error.NotFound($"User with id '{query.UserId}' was not found."));

            // Get distinct menu IDs the user can view through their roles
            var menuIds = await _context.UserRoles
                .Where(ur => ur.UserId == query.UserId)
                .Join(_context.RoleMenus.Where(rm => rm.CanView),
                    ur => ur.RoleId, rm => rm.RoleId, (ur, rm) => rm.MenuId)
                .Distinct()
                .ToListAsync(ct);

            // Fetch those menus
            var menus = await _context.Menus
                .AsNoTracking()
                .Where(m => menuIds.Contains(m.Id) && m.IsActive && !m.IsDeleted)
                .OrderBy(m => m.DisplayOrder)
                .Select(m => new
                {
                    m.Id, m.ParentMenuId, m.MenuCode, m.MenuName, m.DisplayName,
                    m.MenuUrl, m.IconClass, m.DisplayOrder, m.MenuLevel,
                    m.IsExternalLink, m.OpenInNewTab
                })
                .ToListAsync(ct);

            // Build lookup by parent
            var lookup = menus.ToLookup(m => m.ParentMenuId);

            List<MenuNode> BuildChildren(Guid? parentId)
            {
                return lookup[parentId]
                    .Select(m => new MenuNode(
                        m.Id, m.ParentMenuId, m.MenuCode, m.MenuName, m.DisplayName,
                        m.MenuUrl, m.IconClass, m.DisplayOrder, m.MenuLevel,
                        m.IsExternalLink, m.OpenInNewTab, BuildChildren(m.Id)))
                    .ToList();
            }

            var tree = BuildChildren(null);
            return Result<List<MenuNode>>.Success(tree);
        }
    }
}
