using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class AssignRolesToUser
{
    public sealed record Request(List<Guid> RoleIds);
    public sealed record Command(Guid UserId, List<Guid> RoleIds);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.RoleIds).NotNull();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<GetUserById.UserRoleInfo>>> Handle(Command command, CancellationToken ct)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == command.UserId, ct);
            if (!userExists)
                return Result<List<GetUserById.UserRoleInfo>>.Failure(
                    Error.NotFound($"User with id '{command.UserId}' was not found."));

            // Validate all role IDs exist
            if (command.RoleIds.Count > 0)
            {
                var existingRoleIds = await _context.Roles
                    .Where(r => command.RoleIds.Contains(r.Id))
                    .Select(r => r.Id)
                    .ToListAsync(ct);

                var missing = command.RoleIds.Except(existingRoleIds).ToList();
                if (missing.Count > 0)
                    return Result<List<GetUserById.UserRoleInfo>>.Failure(
                        Error.NotFound($"Roles not found: {string.Join(", ", missing)}"));
            }

            // Remove existing roles
            var existing = await _context.UserRoles
                .Where(ur => ur.UserId == command.UserId)
                .ToListAsync(ct);
            _context.UserRoles.RemoveRange(existing);

            // Add new roles
            foreach (var roleId in command.RoleIds)
            {
                _context.UserRoles.Add(new UserRoles
                {
                    UserId = command.UserId,
                    RoleId = roleId
                });
            }

            await _context.SaveChangesAsync(ct);

            // Return updated role list
            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == command.UserId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id,
                    (ur, r) => new GetUserById.UserRoleInfo(r.Id, r.Name))
                .ToListAsync(ct);

            return Result<List<GetUserById.UserRoleInfo>>.Success(roles);
        }
    }
}
