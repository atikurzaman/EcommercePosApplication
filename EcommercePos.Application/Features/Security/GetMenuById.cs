using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
