using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using EcommercePos.Shared.Cryptography;
using Microsoft.Extensions.Configuration;
using FluentValidation;

namespace EcommercePos.Application.Features.Auth;

public static class RegisterUser
{
    public sealed record Request(string Email, string Password, string FirstName, string LastName, string? Phone);
    public sealed record Response(Guid Id, string Email, string FirstName, string LastName);
    public sealed record Command(string Email, string Password, string FirstName, string LastName, string? Phone);
    public sealed class Validator : AbstractValidator<Request> { public Validator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); RuleFor(x => x.Password).NotEmpty().MinimumLength(6); RuleFor(x => x.FirstName).NotEmpty(); RuleFor(x => x.LastName).NotEmpty(); } }
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public Handler(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email, ct);
            if (existingUser != null) return Result<Response>.Failure(Error.Conflict("Email already registered"));
            var user = new Users
            {
                Id = Guid.NewGuid(),
                UserName = command.Email.Split('@')[0] + Guid.NewGuid().ToString("N").Substring(0, 8),
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                PhoneNumber = command.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PreferredLanguage = "en",
                TimeZone = "UTC",
                PasswordHash = _passwordService.HashPassword(command.Password),
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };
            _context.Users.Add(user);
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin" && r.IsActive, ct);
            if (adminRole == null)
            {
                adminRole = new Roles { Id = Guid.NewGuid(), Name = "Admin", IsActive = true, Description = "Administrator role" };
                _context.Roles.Add(adminRole);
                await _context.SaveChangesAsync(ct);
            }
            _context.UserRoles.Add(new UserRoles { UserId = user.Id, RoleId = adminRole.Id });
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(user.Id, user.Email, user.FirstName ?? "", user.LastName ?? ""));
        }
    }
}
