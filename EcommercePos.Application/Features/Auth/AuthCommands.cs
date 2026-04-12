using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
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
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public Handler(ApplicationDbContext context, IConfiguration configuration) { _context = context; _configuration = configuration; }
        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email, ct);
            if (user == null || !VerifyPassword(command.Password, user.PasswordHash)) return Result<Response>.Failure(Error.Unauthorized("Invalid credentials"));
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
        private static bool VerifyPassword(string password, string passwordHash) => HashPassword(password) == passwordHash;
        private static string HashPassword(string password) { using var sha256 = SHA256.Create(); return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password))); }
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

public static class RegisterUser
{
    public sealed record Request(string Email, string Password, string FirstName, string LastName, string? Phone);
    public sealed record Response(Guid Id, string Email, string FirstName, string LastName);
    public sealed record Command(string Email, string Password, string FirstName, string LastName, string? Phone);
    public sealed class Validator : AbstractValidator<Request> { public Validator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.Password).NotEmpty().MinimumLength(6); RuleFor(x => x.FirstName).NotEmpty(); RuleFor(x => x.LastName).NotEmpty(); } }
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) { _context = context; }
        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email, ct);
            if (existingUser != null) return Result<Response>.Failure(Error.Conflict("Email already registered"));
            var user = new Users { Id = Guid.NewGuid(), UserName = command.Email.Split('@')[0] + Guid.NewGuid().ToString("N").Substring(0, 8), Email = command.Email, FirstName = command.FirstName, LastName = command.LastName, PhoneNumber = command.Phone, IsActive = true, CreatedAt = DateTime.UtcNow, PreferredLanguage = "en", TimeZone = "UTC", PasswordHash = HashPassword(command.Password), EmailConfirmed = false, PhoneNumberConfirmed = false, TwoFactorEnabled = false, LockoutEnabled = false, AccessFailedCount = 0 };
            _context.Users.Add(user);
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin" && r.IsActive, ct);
            if (adminRole == null) { adminRole = new Roles { Id = Guid.NewGuid(), Name = "Admin", IsActive = true, Description = "Administrator role" }; _context.Roles.Add(adminRole); await _context.SaveChangesAsync(ct); }
            _context.UserRoles.Add(new UserRoles { UserId = user.Id, RoleId = adminRole.Id });
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(user.Id, user.Email, user.FirstName ?? "", user.LastName ?? ""));
        }
        private static string HashPassword(string password) { using var sha256 = SHA256.Create(); return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password))); }
    }
}

public static class GetCurrentUser
{
    public sealed record Query;
    public sealed record Response(Guid Id, string Email, string FirstName, string LastName, string? PhoneNumber, string? AvatarUrl, List<string> Roles);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) { _context = context; }
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

public static class ChangePassword
{
    public sealed record Request(string CurrentPassword, string NewPassword);
    public sealed record Command(Guid UserId, string CurrentPassword, string NewPassword);
    public sealed class Validator : AbstractValidator<Request> { public Validator() { RuleFor(x => x.CurrentPassword).NotEmpty(); RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6); } }
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) { _context = context; }
        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FindAsync([command.UserId], ct);
            if (user == null) return Result.Failure(Error.NotFound("User not found"));
            if (!VerifyPassword(command.CurrentPassword, user.PasswordHash)) return Result.Failure(Error.Unauthorized("Current password is incorrect"));
            user.PasswordHash = HashPassword(command.NewPassword);
            user.LastPasswordChangedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        private static bool VerifyPassword(string password, string passwordHash) => HashPassword(password) == passwordHash;
        private static string HashPassword(string password) { using var sha256 = SHA256.Create(); return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password))); }
    }
}

public static class AssignRole
{
    public sealed record Request(Guid UserId, Guid RoleId);
    public sealed record Command(Guid UserId, Guid RoleId);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) { _context = context; }
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

public static class RemoveRole
{
    public sealed record Request(Guid UserId, Guid RoleId);
    public sealed record Command(Guid UserId, Guid RoleId);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) { _context = context; }
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

public static class GetRoles
{
    public sealed record Query;
    public sealed record Response(Guid Id, string Name, string? Description, bool IsActive);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) { _context = context; }
        public async Task<List<Response>> Handle(Query query, CancellationToken ct)
        {
            return await _context.Roles.Where(r => r.IsActive).Select(r => new Response(r.Id, r.Name!, r.Description, r.IsActive)).ToListAsync(ct);
        }
    }
}
