using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

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
