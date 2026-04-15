using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.DeliveryZone;

public static class CreateDeliveryZone
{
    public sealed record Command(
        string Name, string? Description, bool IsActive,
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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var zone = new DeliveryZones
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                IsActive = command.IsActive,
                BaseDeliveryCost = command.BaseDeliveryCost,
                FreeDeliveryThreshold = command.FreeDeliveryThreshold,
                MinDeliveryDays = command.MinDeliveryDays,
                MaxDeliveryDays = command.MaxDeliveryDays,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.DeliveryZones.Add(zone);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                zone.Id, zone.Name, zone.IsActive, zone.BaseDeliveryCost));
        }
    }
}
