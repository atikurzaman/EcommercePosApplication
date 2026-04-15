using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.PickupPoint;

public static class GetPickupPointById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, Guid? WarehouseId, string Name, string AddressLine1, string City,
        string? PostalCode, string Phone, decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var point = await _context.PickupPoints
                .Where(p => p.Id == query.Id && !p.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (point is null)
                return Result<Response>.Failure(Error.NotFound($"Pickup point '{query.Id}' was not found."));

            return Result<Response>.Success(new Response(
                point.Id, point.WarehouseId, point.Name, point.AddressLine1, point.City,
                point.PostalCode, point.Phone, point.Latitude, point.Longitude,
                point.OpeningTime, point.ClosingTime, point.IsActive));
        }
    }
}
