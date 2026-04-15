using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.DeliveryZone;

public static class GetDeliveryZoneById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string Name, string? Description, bool IsActive,
        decimal BaseDeliveryCost, decimal? FreeDeliveryThreshold,
        int? MinDeliveryDays, int? MaxDeliveryDays);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var zone = await _context.DeliveryZones
                .Where(z => z.Id == query.Id && !z.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (zone is null)
                return Result<Response>.Failure(Error.NotFound($"Delivery zone '{query.Id}' was not found."));

            return Result<Response>.Success(new Response(
                zone.Id, zone.Name, zone.Description, zone.IsActive,
                zone.BaseDeliveryCost, zone.FreeDeliveryThreshold,
                zone.MinDeliveryDays, zone.MaxDeliveryDays));
        }
    }
}
