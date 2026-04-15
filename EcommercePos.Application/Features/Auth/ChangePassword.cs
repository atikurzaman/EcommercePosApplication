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

public static class ChangePassword
{
    public sealed record Request(string CurrentPassword, string NewPassword);
    public sealed record Command(Guid UserId, string CurrentPassword, string NewPassword);
    public sealed class Validator : AbstractValidator<Request> { public Validator() { RuleFor(x => x.CurrentPassword).NotEmpty(); RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6); } }
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public Handler(IApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FindAsync([command.UserId], ct);
            if (user == null) return Result.Failure(Error.NotFound("User not found"));
            if (!_passwordService.VerifyPassword(command.CurrentPassword, user.PasswordHash))
                return Result.Failure(Error.Unauthorized("Current password is incorrect"));
            user.PasswordHash = _passwordService.HashPassword(command.NewPassword);
            user.LastPasswordChangedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
