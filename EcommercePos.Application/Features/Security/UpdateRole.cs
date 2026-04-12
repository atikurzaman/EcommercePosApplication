using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class UpdateRole
{
    public sealed record Request(string Name, string? Description, bool IsActive);
    public sealed record Command(Guid Id, string Name, string? Description, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetRoles.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == command.Id, ct);

            if (entity == null)
                return Result<GetRoles.Response>.Failure(Error.NotFound("Role not found."));

            if (entity.Name != command.Name)
            {
                var exists = await _context.Roles
                    .AnyAsync(r => r.Name == command.Name && r.Id != command.Id, ct);
                if (exists)
                    return Result<GetRoles.Response>.Failure(
                        Error.Conflict($"Role '{command.Name}' already exists."));
            }

            entity.Name = command.Name;
            entity.NormalizedName = command.Name.ToUpperInvariant();
            entity.Description = command.Description;
            entity.IsActive = command.IsActive;

            await _context.SaveChangesAsync(ct);

            return Result<GetRoles.Response>.Success(
                new GetRoles.Response(entity.Id, entity.Name, entity.Description, entity.IsActive));
        }
    }
}
