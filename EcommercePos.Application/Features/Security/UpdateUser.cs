using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class UpdateUser
{
    public sealed record Request(
        string? FirstName, string? LastName, string? PhoneNumber,
        string? AvatarUrl, bool IsActive, string? PreferredLanguage, string? TimeZone);

    public sealed record Command(
        Guid Id, string? FirstName, string? LastName, string? PhoneNumber,
        string? AvatarUrl, bool IsActive, string? PreferredLanguage, string? TimeZone);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PreferredLanguage).MaximumLength(10);
            RuleFor(x => x.TimeZone).MaximumLength(100);
            RuleFor(x => x.PhoneNumber).MaximumLength(50);
            RuleFor(x => x.FirstName).MaximumLength(100);
            RuleFor(x => x.LastName).MaximumLength(100);
            RuleFor(x => x.AvatarUrl).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetUserById.Response>> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.Id, ct);
            if (user == null)
                return Result<GetUserById.Response>.Failure(Error.NotFound($"User with id '{command.Id}' was not found."));

            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.PhoneNumber = command.PhoneNumber;
            user.AvatarUrl = command.AvatarUrl;
            user.IsActive = command.IsActive;
            user.PreferredLanguage = command.PreferredLanguage ?? user.PreferredLanguage;
            user.TimeZone = command.TimeZone ?? user.TimeZone;

            await _context.SaveChangesAsync(ct);

            // Re-fetch with roles
            var handler = new GetUserById.Handler(_context);
            return await handler.Handle(new GetUserById.Query(command.Id), ct);
        }
    }
}
