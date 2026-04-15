using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class CreatePermission
{
    public sealed record Request(string PermissionCode, string Name, string Module, string? Description, bool IsActive);
    public sealed record Response(Guid Id, string PermissionCode, string Name);

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.Permissions
                .AnyAsync(p => p.PermissionCode == request.PermissionCode && !p.IsDeleted, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Permission with code '{request.PermissionCode}' already exists."));

            var entity = new Permissions
            {
                Id = Guid.NewGuid(),
                PermissionCode = request.PermissionCode,
                Name = request.Name,
                Module = request.Module,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Permissions.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.PermissionCode, entity.Name));
        }
    }
}
