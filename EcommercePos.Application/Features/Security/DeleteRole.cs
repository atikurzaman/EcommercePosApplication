using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class DeleteRole
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == command.Id, ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("Role not found."));

            // Remove related role permissions and role menus first
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == command.Id)
                .ToListAsync(ct);
            _context.RolePermissions.RemoveRange(rolePermissions);

            var roleMenus = await _context.RoleMenus
                .Where(rm => rm.RoleId == command.Id)
                .ToListAsync(ct);
            _context.RoleMenus.RemoveRange(roleMenus);

            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
