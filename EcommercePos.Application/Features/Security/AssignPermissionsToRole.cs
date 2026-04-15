using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class AssignPermissionsToRole
{
    public sealed record PermissionAssignment(Guid PermissionId, bool IsGranted);
    public sealed record Command(Guid RoleId, List<PermissionAssignment> Permissions);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.Permissions).NotNull();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == command.RoleId, ct);
            if (!roleExists)
                return Result.Failure(Error.NotFound("Role not found."));

            // Remove existing permissions for this role
            var existing = await _context.RolePermissions
                .Where(rp => rp.RoleId == command.RoleId)
                .ToListAsync(ct);
            _context.RolePermissions.RemoveRange(existing);

            // Add new permissions
            foreach (var assignment in command.Permissions)
            {
                _context.RolePermissions.Add(new RolePermissions
                {
                    RoleId = command.RoleId,
                    PermissionId = assignment.PermissionId,
                    IsGranted = assignment.IsGranted
                });
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
