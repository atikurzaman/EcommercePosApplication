using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Branch;

public static class CreateBranch
{
    public sealed record Command(
        string WarehouseCode, string Name, string? Description,
        string? AddressLine1, string? AddressLine2, string? City,
        string? Area, string? State, string? PostalCode,
        string? Phone, string? Email, bool IsActive);

    public sealed record Response(Guid Id, string WarehouseCode, string Name, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseCode).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var exists = await _context.Warehouses
                .AnyAsync(w => w.Code == command.WarehouseCode && !w.IsDeleted, ct);

            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Branch with code '{command.WarehouseCode}' already exists."));

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
                Country = "Bangladesh",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Warehouses.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(item.Id, item.Code, item.Name, item.IsActive));
        }
    }
}
