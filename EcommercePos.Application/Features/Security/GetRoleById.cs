using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetRoleById
{
    public sealed record Query(Guid Id);
    public sealed record RolePermissionItem(Guid PermissionId, string PermissionCode, string Name, string Module, bool IsGranted);
    public sealed record RoleMenuItem(Guid MenuId, string MenuCode, string DisplayName, bool CanView, bool CanAdd, bool CanEdit, bool CanDelete, bool CanApprove);
    public sealed record Response(Guid Id, string Name, string? Description, bool IsActive, List<RolePermissionItem> Permissions, List<RoleMenuItem> Menus);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var role = await _context.Roles.AsNoTracking()
                .Where(r => r.Id == query.Id)
                .FirstOrDefaultAsync(ct);

            if (role == null)
                return Result<Response>.Failure(Error.NotFound("Role not found."));

            var permissions = await _context.RolePermissions.AsNoTracking()
                .Where(rp => rp.RoleId == query.Id)
                .Join(_context.Permissions,
                    rp => rp.PermissionId, p => p.Id,
                    (rp, p) => new { p.Id, p.PermissionCode, p.Name, p.Module, rp.IsGranted, p.IsDeleted })
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Module).ThenBy(x => x.Name)
                .Select(x => new RolePermissionItem(x.Id, x.PermissionCode, x.Name, x.Module, x.IsGranted))
                .ToListAsync(ct);

            var menus = await _context.RoleMenus.AsNoTracking()
                .Where(rm => rm.RoleId == query.Id)
                .Join(_context.Menus,
                    rm => rm.MenuId, m => m.Id,
                    (rm, m) => new { m.Id, m.MenuCode, m.DisplayName, rm.CanView, rm.CanAdd, rm.CanEdit, rm.CanDelete, rm.CanApprove, m.IsDeleted })
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.DisplayName)
                .Select(x => new RoleMenuItem(x.Id, x.MenuCode, x.DisplayName, x.CanView, x.CanAdd, x.CanEdit, x.CanDelete, x.CanApprove))
                .ToListAsync(ct);

            return Result<Response>.Success(
                new Response(role.Id, role.Name, role.Description, role.IsActive, permissions, menus));
        }
    }
}
