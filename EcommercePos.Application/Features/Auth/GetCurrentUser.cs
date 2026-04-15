using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;
using Microsoft.Extensions.Configuration;
using FluentValidation;

namespace EcommercePos.Application.Features.Auth;

public static class GetCurrentUser
{
    public sealed record Query;
    public sealed record Response(Guid Id, string Email, string FirstName, string LastName, string? PhoneNumber, string? AvatarUrl, List<string> Roles);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) { _context = context; }
        public async Task<Result<Response>> Handle(Query query, ClaimsPrincipal claims, CancellationToken ct)
        {
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || !Guid.TryParse(userId, out var id)) return Result<Response>.Failure(Error.Unauthorized("Invalid user"));
            var user = await _context.Users.FindAsync([id], ct);
            if (user == null) return Result<Response>.Failure(Error.NotFound("User not found"));
            var roles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!).ToListAsync(ct);
            return Result<Response>.Success(new Response(user.Id, user.Email, user.FirstName ?? "", user.LastName ?? "", user.PhoneNumber, user.AvatarUrl, roles));
        }
    }
}
