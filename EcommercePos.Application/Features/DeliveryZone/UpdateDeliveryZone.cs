using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.DeliveryZone;

public static class UpdateDeliveryZone
{
    public sealed record Command(
        Guid Id, string Name, string? Description, bool IsActive,
        decimal BaseDeliveryCost, decimal? FreeDeliveryThreshold,
        int? MinDeliveryDays, int? MaxDeliveryDays);

    public sealed record Response(
        Guid Id, string Name, bool IsActive, decimal BaseDeliveryCost);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.BaseDeliveryCost).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var zone = await _context.DeliveryZones
                .Where(z => z.Id == command.Id && !z.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (zone is null)
                return Result<Response>.Failure(Error.NotFound($"Delivery zone '{command.Id}' was not found."));

            zone.Name = command.Name;
            zone.Description = command.Description;
            zone.IsActive = command.IsActive;
            zone.BaseDeliveryCost = command.BaseDeliveryCost;
            zone.FreeDeliveryThreshold = command.FreeDeliveryThreshold;
            zone.MinDeliveryDays = command.MinDeliveryDays;
            zone.MaxDeliveryDays = command.MaxDeliveryDays;
            zone.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                zone.Id, zone.Name, zone.IsActive, zone.BaseDeliveryCost));
        }
    }
}
