using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Branch;

public static class UpdateBranch
{
    public sealed record Command(
        Guid Id, string WarehouseCode, string Name, string? Description,
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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Warehouses
                .Where(w => w.Id == command.Id && !w.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Branch '{command.Id}' was not found."));

            var exists = await _context.Warehouses
                .AnyAsync(w => w.Code == command.WarehouseCode && w.Id != command.Id && !w.IsDeleted, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Another branch with code '{command.WarehouseCode}' already exists."));

            item.Code = command.WarehouseCode;
            item.Name = command.Name;
            item.SiteType = command.Description ?? item.SiteType;
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
            return Result<Response>.Success(new Response(item.Id, item.Code, item.Name, item.IsActive));
        }
    }
}
