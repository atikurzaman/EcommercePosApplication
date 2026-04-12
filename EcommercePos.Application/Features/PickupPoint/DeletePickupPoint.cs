using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.PickupPoint;

public static class DeletePickupPoint
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var point = await _context.PickupPoints
                .Where(p => p.Id == command.Id && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (point is null)
                return Result.Failure(Error.NotFound($"Pickup point '{command.Id}' was not found."));

            point.IsDeleted = true;
            point.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
