using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

// ── GetUsers (paginated list with search and filters) ──────────────────────

public static class GetUsers
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null, bool? IsActive = null, Guid? RoleId = null);

    public sealed record Response(
        Guid Id, string UserName, string Email, string? FirstName, string? LastName,
        string? PhoneNumber, bool IsActive, bool EmailConfirmed,
        DateTime CreatedAt, DateTime? LastLoginAt, List<string> Roles);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search;
                query = query.Where(u =>
                    u.UserName.Contains(search) ||
                    u.Email.Contains(search) ||
                    (u.FirstName != null && u.FirstName.Contains(search)) ||
                    (u.LastName != null && u.LastName.Contains(search)));
            }

            if (request.IsActive.HasValue)
                query = query.Where(u => u.IsActive == request.IsActive.Value);

            if (request.RoleId.HasValue)
            {
                var roleId = request.RoleId.Value;
                query = query.Where(u =>
                    _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(u => u.UserName)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new Response(
                    u.Id, u.UserName, u.Email, u.FirstName, u.LastName,
                    u.PhoneNumber, u.IsActive, u.EmailConfirmed,
                    u.CreatedAt, u.LastLoginAt,
                    _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList()))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

// ── GetUserById ────────────────────────────────────────────────────────────

public static class GetUserById
{
    public sealed record Query(Guid Id);

    public sealed record UserRoleInfo(Guid RoleId, string RoleName);

    public sealed record Response(
        Guid Id, string UserName, string Email, string? FirstName, string? LastName,
        string? PhoneNumber, string? AvatarUrl, bool IsActive, bool EmailConfirmed,
        bool TwoFactorEnabled, string PreferredLanguage, string TimeZone,
        DateTime CreatedAt, DateTime? LastLoginAt, List<UserRoleInfo> Roles);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var user = await _context.Users.AsNoTracking()
                .Where(u => u.Id == query.Id)
                .Select(u => new
                {
                    u.Id, u.UserName, u.Email, u.FirstName, u.LastName,
                    u.PhoneNumber, u.AvatarUrl, u.IsActive, u.EmailConfirmed,
                    u.TwoFactorEnabled, u.PreferredLanguage, u.TimeZone,
                    u.CreatedAt, u.LastLoginAt,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new UserRoleInfo(r.Id, r.Name))
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (user == null)
                return Result<Response>.Failure(Error.NotFound($"User with id '{query.Id}' was not found."));

            return Result<Response>.Success(new Response(
                user.Id, user.UserName, user.Email, user.FirstName, user.LastName,
                user.PhoneNumber, user.AvatarUrl, user.IsActive, user.EmailConfirmed,
                user.TwoFactorEnabled, user.PreferredLanguage, user.TimeZone,
                user.CreatedAt, user.LastLoginAt, user.Roles));
        }
    }
}

// ── UpdateUser (profile fields only, not password) ─────────────────────────

public static class UpdateUser
{
    public sealed record Request(
        string? FirstName, string? LastName, string? PhoneNumber,
        string? AvatarUrl, bool IsActive, string PreferredLanguage, string TimeZone);

    public sealed record Command(
        Guid Id, string? FirstName, string? LastName, string? PhoneNumber,
        string? AvatarUrl, bool IsActive, string PreferredLanguage, string TimeZone);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PreferredLanguage).NotEmpty().MaximumLength(10);
            RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PhoneNumber).MaximumLength(50);
            RuleFor(x => x.FirstName).MaximumLength(100);
            RuleFor(x => x.LastName).MaximumLength(100);
            RuleFor(x => x.AvatarUrl).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetUserById.Response>> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.Id, ct);
            if (user == null)
                return Result<GetUserById.Response>.Failure(Error.NotFound($"User with id '{command.Id}' was not found."));

            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.PhoneNumber = command.PhoneNumber;
            user.AvatarUrl = command.AvatarUrl;
            user.IsActive = command.IsActive;
            user.PreferredLanguage = command.PreferredLanguage;
            user.TimeZone = command.TimeZone;

            await _context.SaveChangesAsync(ct);

            // Re-fetch with roles
            var handler = new GetUserById.Handler(_context);
            return await handler.Handle(new GetUserById.Query(command.Id), ct);
        }
    }
}

// ── ToggleUserActive ───────────────────────────────────────────────────────

public static class ToggleUserActive
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<bool>> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.Id, ct);
            if (user == null)
                return Result<bool>.Failure(Error.NotFound($"User with id '{command.Id}' was not found."));

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync(ct);

            return Result<bool>.Success(user.IsActive);
        }
    }
}

// ── AssignRolesToUser (bulk replace) ───────────────────────────────────────

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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

// ── GetUserMenus (tree of menus the user can view via their roles) ─────────

public static class GetUserMenus
{
    public sealed record Query(Guid UserId);

    public sealed record MenuNode(
        Guid Id, Guid? ParentMenuId, string MenuCode, string MenuName, string DisplayName,
        string? MenuUrl, string? IconClass, int DisplayOrder, byte MenuLevel,
        bool IsExternalLink, bool OpenInNewTab, List<MenuNode> Children);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<MenuNode>>> Handle(Query query, CancellationToken ct)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == query.UserId, ct);
            if (!userExists)
                return Result<List<MenuNode>>.Failure(Error.NotFound($"User with id '{query.UserId}' was not found."));

            // Get distinct menu IDs the user can view through their roles
            var menuIds = await _context.UserRoles
                .Where(ur => ur.UserId == query.UserId)
                .Join(_context.RoleMenus.Where(rm => rm.CanView),
                    ur => ur.RoleId, rm => rm.RoleId, (ur, rm) => rm.MenuId)
                .Distinct()
                .ToListAsync(ct);

            // Fetch those menus
            var menus = await _context.Menus
                .AsNoTracking()
                .Where(m => menuIds.Contains(m.Id) && m.IsActive && !m.IsDeleted)
                .OrderBy(m => m.DisplayOrder)
                .Select(m => new
                {
                    m.Id, m.ParentMenuId, m.MenuCode, m.MenuName, m.DisplayName,
                    m.MenuUrl, m.IconClass, m.DisplayOrder, m.MenuLevel,
                    m.IsExternalLink, m.OpenInNewTab
                })
                .ToListAsync(ct);

            // Build lookup by parent
            var lookup = menus.ToLookup(m => m.ParentMenuId);

            List<MenuNode> BuildChildren(Guid? parentId)
            {
                return lookup[parentId]
                    .Select(m => new MenuNode(
                        m.Id, m.ParentMenuId, m.MenuCode, m.MenuName, m.DisplayName,
                        m.MenuUrl, m.IconClass, m.DisplayOrder, m.MenuLevel,
                        m.IsExternalLink, m.OpenInNewTab, BuildChildren(m.Id)))
                    .ToList();
            }

            var tree = BuildChildren(null);
            return Result<List<MenuNode>>.Success(tree);
        }
    }
}

// ── GetUserPermissions (distinct permission codes for a user) ──────────────

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
