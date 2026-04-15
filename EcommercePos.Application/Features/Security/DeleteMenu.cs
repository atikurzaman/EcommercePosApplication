using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Security;

public static class DeleteMenu
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Menus
                .Where(m => m.Id == command.Id && !m.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("Menu not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
