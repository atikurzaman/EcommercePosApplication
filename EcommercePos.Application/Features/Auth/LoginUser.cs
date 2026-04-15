using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;
using EcommercePos.Shared.Cryptography;
using Microsoft.Extensions.Configuration;
using FluentValidation;

namespace EcommercePos.Application.Features.Auth;

public static class LoginUser
{
    public sealed record Request(string Email, string Password);
    public sealed record Response(string AccessToken, string RefreshToken, Guid UserId, string Email, string FirstName, string LastName, List<string> Roles);
    public sealed record Command(string Email, string Password);
    public sealed class Validator : AbstractValidator<Request> { public Validator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.Password).NotEmpty(); } }
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService;

        public Handler(IApplicationDbContext context, IConfiguration configuration, IPasswordService passwordService)
        {
            _context = context;
            _configuration = configuration;
            _passwordService = passwordService;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email, ct);
            if (user == null || !_passwordService.VerifyPassword(command.Password, user.PasswordHash))
                return Result<Response>.Failure(Error.Unauthorized("Invalid credentials"));
            if (!user.IsActive) return Result<Response>.Failure(Error.Unauthorized("Account is disabled"));
            var userRoles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).Join(_context.Roles.Where(r => r.IsActive), ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!).ToListAsync(ct);
            if (!userRoles.Any()) userRoles = ["Admin"];
            var roleIds = await _context.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToListAsync(ct);
            var permissions = await _context.RolePermissions.Where(rp => roleIds.Contains(rp.RoleId) && rp.IsGranted).Join(_context.Permissions.Where(p => p.IsActive && !p.IsDeleted), rp => rp.PermissionId, p => p.Id, (rp, p) => p.PermissionCode).Distinct().ToListAsync(ct);
            var token = GenerateJwtToken(user, userRoles, permissions, _configuration);
            var refreshToken = GenerateRefreshToken();
            _context.UserRefreshTokens.Add(new UserRefreshTokens { Id = Guid.NewGuid(), UserId = user.Id, Token = refreshToken, ExpiresAt = DateTime.UtcNow.AddDays(7), CreatedAt = DateTime.UtcNow });
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(token, refreshToken, user.Id, user.Email, user.FirstName ?? "", user.LastName ?? "", userRoles));
        }

        private static string GenerateJwtToken(Users user, IEnumerable<string> roles, IEnumerable<string> permissions, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()), new(ClaimTypes.Email, user.Email), new(ClaimTypes.Name, user.FirstName ?? ""), new(ClaimTypes.Surname, user.LastName ?? ""), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };
            foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
            foreach (var permission in permissions) claims.Add(new Claim("permissions", permission));
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(issuer: jwtSettings["Issuer"], audience: jwtSettings["Audience"], claims: claims, expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryInMinutes"]!)), signingCredentials: credentials));
        }

        private static string GenerateRefreshToken() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
