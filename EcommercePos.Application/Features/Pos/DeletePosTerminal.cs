using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── DeletePosTerminal ──────────────────────────────────────────────────────────
public static class DeletePosTerminal
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PosTerminals
                .FirstOrDefaultAsync(t => t.Id == command.Id && !t.IsDeleted, ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("POS terminal not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
