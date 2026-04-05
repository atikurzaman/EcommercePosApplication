using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Branch.Commands;

public static class CreateBranch
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
        string WarehouseCode, string Name, string? Description, string? AddressLine1, 
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
            var exists = await _context.Warehouses
                .AnyAsync(x => x.Code == command.WarehouseCode && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Warehouse with WarehouseCode '{command.WarehouseCode}' already exists."));
            }

            var item = new Warehouses
            {
                Id = Guid.NewGuid(),
                Code = command.WarehouseCode,
                Name = command.Name,
                SiteType = command.Description ?? string.Empty,
                AddressLine1 = command.AddressLine1,
                AddressLine2 = command.AddressLine2,
                City = command.City,
                Area = command.Area,
                State = command.State,
                PostalCode = command.PostalCode,
                Phone = command.Phone,
                Email = command.Email,
                IsActive = command.IsActive,
                Country = "Thailand",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Warehouses.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}