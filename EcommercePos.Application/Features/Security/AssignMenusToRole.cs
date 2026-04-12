using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class AssignMenusToRole
{
    public sealed record MenuAssignment(Guid MenuId, bool CanView, bool CanAdd, bool CanEdit, bool CanDelete, bool CanApprove);
    public sealed record Command(Guid RoleId, List<MenuAssignment> Menus);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
            RuleFor(x => x.Menus).NotNull();
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == command.RoleId, ct);
            if (!roleExists)
                return Result.Failure(Error.NotFound("Role not found."));

            // Remove existing menu assignments for this role
            var existing = await _context.RoleMenus
                .Where(rm => rm.RoleId == command.RoleId)
                .ToListAsync(ct);
            _context.RoleMenus.RemoveRange(existing);

            // Add new menu assignments
            foreach (var assignment in command.Menus)
            {
                _context.RoleMenus.Add(new RoleMenus
                {
                    Id = Guid.NewGuid(),
                    RoleId = command.RoleId,
                    MenuId = assignment.MenuId,
                    CanView = assignment.CanView,
                    CanAdd = assignment.CanAdd,
                    CanEdit = assignment.CanEdit,
                    CanDelete = assignment.CanDelete,
                    CanApprove = assignment.CanApprove,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
