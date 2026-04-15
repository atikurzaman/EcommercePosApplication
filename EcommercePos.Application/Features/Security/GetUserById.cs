using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
