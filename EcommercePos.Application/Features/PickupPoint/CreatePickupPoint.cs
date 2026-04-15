using FluentValidation;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.PickupPoint;

public static class CreatePickupPoint
{
    public sealed record Command(
        Guid? WarehouseId, string Name, string AddressLine1, string City,
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
            RuleFor(x => x.Phone).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var point = new PickupPoints
            {
                Id = Guid.NewGuid(),
                WarehouseId = command.WarehouseId,
                Name = command.Name,
                AddressLine1 = command.AddressLine1,
                City = command.City,
                PostalCode = command.PostalCode,
                Phone = command.Phone,
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                OpeningTime = command.OpeningTime,
                ClosingTime = command.ClosingTime,
                IsActive = command.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.PickupPoints.Add(point);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(point.Id, point.Name, point.City, point.IsActive));
        }
    }
}
