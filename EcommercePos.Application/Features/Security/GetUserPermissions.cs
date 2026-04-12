using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetUserPermissions
{
    public sealed record Query(Guid UserId);
    public sealed record Response(List<string> PermissionCodes);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == query.UserId, ct);
            if (!userExists)
                return Result<Response>.Failure(Error.NotFound($"User with id '{query.UserId}' was not found."));

            var permissionCodes = await _context.UserRoles
                .Where(ur => ur.UserId == query.UserId)
                .Join(_context.RolePermissions.Where(rp => rp.IsGranted),
                    ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp.PermissionId)
                .Distinct()
                .Join(_context.Permissions.Where(p => p.IsActive && !p.IsDeleted),
                    pid => pid, p => p.Id, (pid, p) => p.PermissionCode)
                .Distinct()
                .OrderBy(code => code)
                .ToListAsync(ct);

            return Result<Response>.Success(new Response(permissionCodes));
        }
    }
}
