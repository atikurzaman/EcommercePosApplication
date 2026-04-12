using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── UpdateWarehouse ────────────────────────────────────────────────────────────
public static class UpdateWarehouse
{
    public sealed record Request(
        string Code, string Name, string SiteType,
        Guid? ParentId, string? ContactPerson, string? ManagerName,
        string? AddressLine1, string? AddressLine2, string? City, string? Area,
        string? State, string? PostalCode, string Country,
        string? Phone, string? Email,
        decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime,
        string? TaxNumber, bool IsDefault, bool IsActive);

    public sealed record Command(
        Guid Id, string Code, string Name, string SiteType,
        Guid? ParentId, string? ContactPerson, string? ManagerName,
        string? AddressLine1, string? AddressLine2, string? City, string? Area,
        string? State, string? PostalCode, string Country,
        string? Phone, string? Email,
        decimal? Latitude, decimal? Longitude,
        TimeOnly? OpeningTime, TimeOnly? ClosingTime,
        string? TaxNumber, bool IsDefault, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.SiteType).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(80);
            RuleFor(x => x.Email).MaximumLength(150);
            RuleFor(x => x.Phone).MaximumLength(30);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetWarehouseById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == command.Id && !w.IsDeleted, ct);

            if (entity == null)
                return Result<GetWarehouseById.Response>.Failure(Error.NotFound("Warehouse not found."));

            if (entity.Code != command.Code)
            {
                var codeExists = await _context.Warehouses
                    .AnyAsync(w => w.Code == command.Code && w.Id != command.Id && !w.IsDeleted, ct);
                if (codeExists)
                    return Result<GetWarehouseById.Response>.Failure(
                        Error.Conflict($"Warehouse code '{command.Code}' already exists."));
            }

            entity.Code = command.Code;
            entity.Name = command.Name;
            entity.SiteType = command.SiteType;
            entity.ParentId = command.ParentId;
            entity.ContactPerson = command.ContactPerson;
            entity.ManagerName = command.ManagerName;
            entity.AddressLine1 = command.AddressLine1;
            entity.AddressLine2 = command.AddressLine2;
            entity.City = command.City;
            entity.Area = command.Area;
            entity.State = command.State;
            entity.PostalCode = command.PostalCode;
            entity.Country = command.Country;
            entity.Phone = command.Phone;
            entity.Email = command.Email;
            entity.Latitude = command.Latitude;
            entity.Longitude = command.Longitude;
            entity.OpeningTime = command.OpeningTime;
            entity.ClosingTime = command.ClosingTime;
            entity.TaxNumber = command.TaxNumber;
            entity.IsDefault = command.IsDefault;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var counters = await _context.PosCounters
                .AsNoTracking()
                .Where(c => c.WarehouseId == entity.Id && !c.IsDeleted)
                .OrderBy(c => c.CounterCode)
                .Select(c => new GetWarehouseById.PosCounterInfo(c.Id, c.CounterCode, c.CounterName, c.IsActive))
                .ToListAsync(ct);

            return Result<GetWarehouseById.Response>.Success(
                new GetWarehouseById.Response(
                    entity.Id, entity.Code, entity.Name, entity.SiteType,
                    entity.ParentId, entity.ContactPerson, entity.ManagerName,
                    entity.AddressLine1, entity.AddressLine2, entity.City, entity.Area,
                    entity.State, entity.PostalCode, entity.Country,
                    entity.Phone, entity.Email,
                    entity.Latitude, entity.Longitude,
                    entity.OpeningTime, entity.ClosingTime,
                    entity.TaxNumber, entity.IsDefault, entity.IsActive, counters));
        }
    }
}
