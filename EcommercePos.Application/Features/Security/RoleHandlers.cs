using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetRoles
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(Guid Id, string Name, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Roles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(r => r.Name.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(r => r.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new Response(r.Id, r.Name, r.Description, r.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

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

public static class CreateRole
{
    public sealed record Request(string Name, string? Description, bool IsActive);
    public sealed record Response(Guid Id, string Name);

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

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.Roles
                .AnyAsync(r => r.Name == request.Name, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Role '{request.Name}' already exists."));

            var entity = new Roles
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                NormalizedName = request.Name.ToUpperInvariant(),
                Description = request.Description,
                IsActive = request.IsActive,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            _context.Roles.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Name));
        }
    }
}

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

public static class DeleteRole
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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
