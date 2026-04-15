using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.DeliveryZone;

public static class DeleteDeliveryZone
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var zone = await _context.DeliveryZones
                .Where(z => z.Id == command.Id && !z.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (zone is null)
                return Result.Failure(Error.NotFound($"Delivery zone '{command.Id}' was not found."));

            zone.IsDeleted = true;
            zone.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
