using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Branch.Commands;

public static class UpdateBranch
{
    public sealed record Request
    {
        public string WarehouseCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string? City { get; init; }
        public string? Area { get; init; }
        public string? State { get; init; }
        public string? PostalCode { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string WarehouseCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string? City { get; init; }
        public string? Area { get; init; }
        public string? State { get; init; }
        public string? PostalCode { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(
        Guid Id, string WarehouseCode, string Name, string? Description, string? AddressLine1, 
        string? AddressLine2, string? City, string? Area, string? State, string? PostalCode, 
        string? Phone, string? Email, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator() {
            RuleFor(x => x.WarehouseCode).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Warehouses
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Warehouse with id '{command.Id}' was not found."));
            }

            var exists = await _context.Warehouses
                .AnyAsync(x => x.Code == command.WarehouseCode && x.Id != command.Id && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Another Warehouse with WarehouseCode '{command.WarehouseCode}' already exists."));
            }

            item.Code = command.WarehouseCode;
            item.Name = command.Name;
            item.SiteType = command.Description ?? string.Empty;
            item.AddressLine1 = command.AddressLine1;
            item.AddressLine2 = command.AddressLine2;
            item.City = command.City;
            item.Area = command.Area;
            item.State = command.State;
            item.PostalCode = command.PostalCode;
            item.Phone = command.Phone;
            item.Email = command.Email;
            item.IsActive = command.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}