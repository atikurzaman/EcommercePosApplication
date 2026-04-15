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

public static class AssignRole
{
    public sealed record Request(Guid UserId, Guid RoleId);
    public sealed record Command(Guid UserId, Guid RoleId);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) { _context = context; }
        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FindAsync([command.UserId], ct);
            if (user == null) return Result.Failure(Error.NotFound("User not found"));
            var role = await _context.Roles.FindAsync([command.RoleId], ct);
            if (role == null) return Result.Failure(Error.NotFound("Role not found"));
            var existing = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId, ct);
            if (existing != null) return Result.Success();
            _context.UserRoles.Add(new UserRoles { UserId = command.UserId, RoleId = command.RoleId });
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
