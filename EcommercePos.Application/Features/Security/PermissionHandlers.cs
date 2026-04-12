using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetPermissions
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null, string? Module = null);
    public sealed record Response(Guid Id, string PermissionCode, string Name, string Module, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Permissions
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(p =>
                    p.PermissionCode.Contains(request.Search) ||
                    p.Name.Contains(request.Search) ||
                    p.Module.Contains(request.Search));

            if (!string.IsNullOrWhiteSpace(request.Module))
                query = query.Where(p => p.Module == request.Module);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new Response(p.Id, p.PermissionCode, p.Name, p.Module, p.Description, p.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetPermissionById
{
    public sealed record Query(Guid Id);
    public sealed record Response(Guid Id, string PermissionCode, string Name, string Module, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Permissions.AsNoTracking()
                .Where(p => p.Id == query.Id && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Permission not found."));

            return Result<Response>.Success(
                new Response(entity.Id, entity.PermissionCode, entity.Name, entity.Module, entity.Description, entity.IsActive));
        }
    }
}

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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

public static class DeletePermission
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Permissions
                .Where(p => p.Id == command.Id && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("Permission not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}

public static class GetPermissionModules
{
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<string>>> Handle(CancellationToken ct)
        {
            var modules = await _context.Permissions
                .Where(p => !p.IsDeleted && p.IsActive)
                .Select(p => p.Module)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync(ct);

            return Result<List<string>>.Success(modules);
        }
    }
}
