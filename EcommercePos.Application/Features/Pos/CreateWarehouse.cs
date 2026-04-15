using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── CreateWarehouse ────────────────────────────────────────────────────────────
public static class CreateWarehouse
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

    public sealed record Response(Guid Id, string Code, string Name);

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var codeExists = await _context.Warehouses
                .AnyAsync(w => w.Code == request.Code && !w.IsDeleted, ct);
            if (codeExists)
                return Result<Response>.Failure(
                    Error.Conflict($"Warehouse code '{request.Code}' already exists."));

            var entity = new Warehouses
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                SiteType = request.SiteType,
                ParentId = request.ParentId,
                ContactPerson = request.ContactPerson,
                ManagerName = request.ManagerName,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Area = request.Area,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Phone = request.Phone,
                Email = request.Email,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                OpeningTime = request.OpeningTime,
                ClosingTime = request.ClosingTime,
                TaxNumber = request.TaxNumber,
                IsDefault = request.IsDefault,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Warehouses.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Code, entity.Name));
        }
    }
}
