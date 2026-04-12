using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class GetMenus
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(
        Guid Id, string MenuCode, string MenuName, string DisplayName, string? MenuUrl,
        string? IconClass, int DisplayOrder, byte MenuLevel, string? PermissionCode,
        Guid? ParentMenuId, bool IsActive, bool IsVisible);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Menus
                .Where(m => !m.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(m =>
                    m.MenuCode.Contains(request.Search) ||
                    m.MenuName.Contains(request.Search) ||
                    m.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(m => m.DisplayOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new Response(
                    m.Id, m.MenuCode, m.MenuName, m.DisplayName, m.MenuUrl,
                    m.IconClass, m.DisplayOrder, m.MenuLevel, m.PermissionCode,
                    m.ParentMenuId, m.IsActive, m.IsVisible))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetMenuTree
{
    public sealed record MenuTreeItem(
        Guid Id, string MenuCode, string DisplayName, string? MenuUrl,
        string? IconClass, int DisplayOrder, byte MenuLevel,
        bool IsActive, bool IsVisible, List<MenuTreeItem> Children);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<MenuTreeItem>>> Handle(CancellationToken ct)
        {
            var allMenus = await _context.Menus
                .Where(m => !m.IsDeleted)
                .AsNoTracking()
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(ct);

            var lookup = allMenus.ToLookup(m => m.ParentMenuId);

            List<MenuTreeItem> BuildChildren(Guid? parentId)
            {
                return lookup[parentId]
                    .Select(m => new MenuTreeItem(
                        m.Id, m.MenuCode, m.DisplayName, m.MenuUrl,
                        m.IconClass, m.DisplayOrder, m.MenuLevel,
                        m.IsActive, m.IsVisible, BuildChildren(m.Id)))
                    .ToList();
            }

            var tree = BuildChildren(null);
            return Result<List<MenuTreeItem>>.Success(tree);
        }
    }
}

public static class GetMenuById
{
    public sealed record Query(Guid Id);
    public sealed record Response(
        Guid Id, string MenuCode, string MenuName, string DisplayName, string? MenuUrl,
        string? IconClass, int DisplayOrder, byte MenuLevel, string? PermissionCode,
        Guid? ParentMenuId, bool IsActive, bool IsVisible, bool IsExternalLink,
        bool OpenInNewTab, string? Description);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Menus.AsNoTracking()
                .Where(m => m.Id == query.Id && !m.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Menu not found."));

            return Result<Response>.Success(new Response(
                entity.Id, entity.MenuCode, entity.MenuName, entity.DisplayName, entity.MenuUrl,
                entity.IconClass, entity.DisplayOrder, entity.MenuLevel, entity.PermissionCode,
                entity.ParentMenuId, entity.IsActive, entity.IsVisible, entity.IsExternalLink,
                entity.OpenInNewTab, entity.Description));
        }
    }
}

public static class CreateMenu
{
    public sealed record Request(
        string MenuCode, string MenuName, string DisplayName, string? MenuUrl,
        string? IconClass, int DisplayOrder, byte MenuLevel, string? PermissionCode,
        Guid? ParentMenuId, bool IsActive, bool IsVisible, bool IsExternalLink,
        bool OpenInNewTab, string? Description);

    public sealed record Response(Guid Id, string MenuCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.MenuCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.MenuName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.MenuUrl).MaximumLength(500);
            RuleFor(x => x.IconClass).MaximumLength(100);
            RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.Menus
                .AnyAsync(m => m.MenuCode == request.MenuCode && !m.IsDeleted, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Menu with code '{request.MenuCode}' already exists."));

            var entity = new Menus
            {
                Id = Guid.NewGuid(),
                MenuCode = request.MenuCode,
                MenuName = request.MenuName,
                DisplayName = request.DisplayName,
                MenuUrl = request.MenuUrl,
                IconClass = request.IconClass,
                DisplayOrder = request.DisplayOrder,
                MenuLevel = request.MenuLevel,
                PermissionCode = request.PermissionCode,
                ParentMenuId = request.ParentMenuId,
                IsActive = request.IsActive,
                IsVisible = request.IsVisible,
                IsExternalLink = request.IsExternalLink,
                OpenInNewTab = request.OpenInNewTab,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Menus.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.MenuCode, entity.DisplayName));
        }
    }
}

public static class UpdateMenu
{
    public sealed record Request(
        string MenuCode, string MenuName, string DisplayName, string? MenuUrl,
        string? IconClass, int DisplayOrder, byte MenuLevel, string? PermissionCode,
        Guid? ParentMenuId, bool IsActive, bool IsVisible, bool IsExternalLink,
        bool OpenInNewTab, string? Description);

    public sealed record Command(
        Guid Id, string MenuCode, string MenuName, string DisplayName, string? MenuUrl,
        string? IconClass, int DisplayOrder, byte MenuLevel, string? PermissionCode,
        Guid? ParentMenuId, bool IsActive, bool IsVisible, bool IsExternalLink,
        bool OpenInNewTab, string? Description);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.MenuCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.MenuName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.MenuUrl).MaximumLength(500);
            RuleFor(x => x.IconClass).MaximumLength(100);
            RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetMenuById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Menus
                .Where(m => m.Id == command.Id && !m.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<GetMenuById.Response>.Failure(Error.NotFound("Menu not found."));

            if (entity.MenuCode != command.MenuCode)
            {
                var exists = await _context.Menus
                    .AnyAsync(m => m.MenuCode == command.MenuCode && m.Id != command.Id && !m.IsDeleted, ct);
                if (exists)
                    return Result<GetMenuById.Response>.Failure(
                        Error.Conflict($"Menu with code '{command.MenuCode}' already exists."));
            }

            entity.MenuCode = command.MenuCode;
            entity.MenuName = command.MenuName;
            entity.DisplayName = command.DisplayName;
            entity.MenuUrl = command.MenuUrl;
            entity.IconClass = command.IconClass;
            entity.DisplayOrder = command.DisplayOrder;
            entity.MenuLevel = command.MenuLevel;
            entity.PermissionCode = command.PermissionCode;
            entity.ParentMenuId = command.ParentMenuId;
            entity.IsActive = command.IsActive;
            entity.IsVisible = command.IsVisible;
            entity.IsExternalLink = command.IsExternalLink;
            entity.OpenInNewTab = command.OpenInNewTab;
            entity.Description = command.Description;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<GetMenuById.Response>.Success(new GetMenuById.Response(
                entity.Id, entity.MenuCode, entity.MenuName, entity.DisplayName, entity.MenuUrl,
                entity.IconClass, entity.DisplayOrder, entity.MenuLevel, entity.PermissionCode,
                entity.ParentMenuId, entity.IsActive, entity.IsVisible, entity.IsExternalLink,
                entity.OpenInNewTab, entity.Description));
        }
    }
}

public static class DeleteMenu
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Menus
                .Where(m => m.Id == command.Id && !m.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("Menu not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
