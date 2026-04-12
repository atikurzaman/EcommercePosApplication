using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class ToggleUserActive
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<bool>> Handle(Command command, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.Id, ct);
            if (user == null)
                return Result<bool>.Failure(Error.NotFound($"User with id '{command.Id}' was not found."));

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync(ct);

            return Result<bool>.Success(user.IsActive);
        }
    }
}
