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

public static class RemoveRole
{
    public sealed record Request(Guid UserId, Guid RoleId);
    public sealed record Command(Guid UserId, Guid RoleId);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) { _context = context; }
        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId, ct);
            if (userRole == null) return Result.Failure(Error.NotFound("User role not found"));
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
