using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateShipmentStatus
{
    public sealed record Request(string StatusCode, string DisplayName, byte SortOrder);
    public sealed record Command(string OriginalCode, string StatusCode, string DisplayName, byte SortOrder);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetShipmentStatusByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ShipmentStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetShipmentStatusByCode.Response>.Failure(Error.NotFound("Shipment status not found."));

            if (entity.StatusCode != command.StatusCode)
            {
                var exists = await _context.ShipmentStatuses.AnyAsync(c => c.StatusCode == command.StatusCode, ct);
                if (exists)
                    return Result<GetShipmentStatusByCode.Response>.Failure(Error.Conflict($"Shipment status '{command.StatusCode}' already exists."));
            }

            entity.StatusCode = command.StatusCode;
            entity.DisplayName = command.DisplayName;
            entity.SortOrder = command.SortOrder;

            await _context.SaveChangesAsync(ct);
            return Result<GetShipmentStatusByCode.Response>.Success(
                new GetShipmentStatusByCode.Response(entity.StatusCode, entity.DisplayName, entity.SortOrder));
        }
    }
}
