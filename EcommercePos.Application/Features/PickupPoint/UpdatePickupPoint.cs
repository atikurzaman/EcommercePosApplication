using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.PickupPoint;

public static class UpdatePickupPoint
{
    public sealed record Command(
        Guid Id, Guid? WarehouseId, string Name, string AddressLine1, string City,
        string? PostalCode, string Phone, decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime, bool IsActive);

    public sealed record Response(Guid Id, string Name, string City, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.AddressLine1).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var point = await _context.PickupPoints
                .Where(p => p.Id == command.Id && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (point is null)
                return Result<Response>.Failure(Error.NotFound($"Pickup point '{command.Id}' was not found."));

            point.WarehouseId = command.WarehouseId;
            point.Name = command.Name;
            point.AddressLine1 = command.AddressLine1;
            point.City = command.City;
            point.PostalCode = command.PostalCode;
            point.Phone = command.Phone;
            point.Latitude = command.Latitude;
            point.Longitude = command.Longitude;
            point.OpeningTime = command.OpeningTime;
            point.ClosingTime = command.ClosingTime;
            point.IsActive = command.IsActive;
            point.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(point.Id, point.Name, point.City, point.IsActive));
        }
    }
}
