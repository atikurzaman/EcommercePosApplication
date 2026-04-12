using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class UpdatePermission
{
    public sealed record Request(string PermissionCode, string Name, string Module, string? Description, bool IsActive);
    public sealed record Command(Guid Id, string PermissionCode, string Name, string Module, string? Description, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PermissionCode).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Module).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetPermissionById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Permissions
                .Where(p => p.Id == command.Id && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<GetPermissionById.Response>.Failure(Error.NotFound("Permission not found."));

            if (entity.PermissionCode != command.PermissionCode)
            {
                var exists = await _context.Permissions
                    .AnyAsync(p => p.PermissionCode == command.PermissionCode && p.Id != command.Id && !p.IsDeleted, ct);
                if (exists)
                    return Result<GetPermissionById.Response>.Failure(
                        Error.Conflict($"Permission with code '{command.PermissionCode}' already exists."));
            }

            entity.PermissionCode = command.PermissionCode;
            entity.Name = command.Name;
            entity.Module = command.Module;
            entity.Description = command.Description;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<GetPermissionById.Response>.Success(
                new GetPermissionById.Response(entity.Id, entity.PermissionCode, entity.Name, entity.Module, entity.Description, entity.IsActive));
        }
    }
}
