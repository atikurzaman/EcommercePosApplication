using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class DeleteShipmentStatus
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ShipmentStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Shipment status not found."));

            _context.ShipmentStatuses.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
