using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

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
