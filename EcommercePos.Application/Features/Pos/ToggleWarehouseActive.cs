using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── ToggleWarehouseActive ──────────────────────────────────────────────────────
public static class ToggleWarehouseActive
{
    public sealed record Command(Guid Id);

    public sealed record Response(Guid Id, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == command.Id && !w.IsDeleted, ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found."));

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.IsActive));
        }
    }
}
