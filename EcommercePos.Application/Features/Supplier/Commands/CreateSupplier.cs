using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using FluentValidation;

namespace EcommercePos.Application.Features.Supplier.Commands;

public static class CreateSupplier
{
    public sealed record Request
    {
        public string SupplierCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? CompanyName { get; init; }
        public string? ContactPerson { get; init; }
        public string? Phone { get; init; }
        public string? AlternatePhone { get; init; }
        public string? Email { get; init; }
        public string? AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string? City { get; init; }
        public string? State { get; init; }
        public string? PostalCode { get; init; }
        public string Country { get; init; } = string.Empty;
        public string? SupplierType { get; init; }
        public string? TaxRegistrationNo { get; init; }
        public string? PaymentTerms { get; init; }
        public int? LeadTimeDays { get; init; }
        public string? Notes { get; init; }
        public bool IsActive { get; init; } = true;
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string SupplierCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? CompanyName { get; init; }
        public string? ContactPerson { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(
        string SupplierCode, string Name, string? CompanyName, string? ContactPerson,
        string? Phone, string? AlternatePhone, string? Email, string? AddressLine1,
        string? AddressLine2, string? City, string? State, string? PostalCode,
        string Country, string? SupplierType, string? TaxRegistrationNo,
        string? PaymentTerms, int? LeadTimeDays, string? Notes, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.SupplierCode).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
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
            var exists = await _context.Suppliers
                .AnyAsync(x => x.SupplierCode == command.SupplierCode && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Supplier with SupplierCode '{command.SupplierCode}' already exists."));
            }

            var item = new Suppliers
            {
                SupplierCode = command.SupplierCode,
                Name = command.Name,
                CompanyName = command.CompanyName,
                ContactPerson = command.ContactPerson,
                Phone = command.Phone,
                AlternatePhone = command.AlternatePhone,
                Email = command.Email,
                AddressLine1 = command.AddressLine1,
                AddressLine2 = command.AddressLine2,
                City = command.City,
                State = command.State,
                PostalCode = command.PostalCode,
                Country = command.Country,
                SupplierType = command.SupplierType,
                TaxRegistrationNo = command.TaxRegistrationNo,
                PaymentTerms = command.PaymentTerms,
                LeadTimeDays = command.LeadTimeDays,
                Notes = command.Notes,
                IsActive = command.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Suppliers.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                SupplierCode = item.SupplierCode,
                Name = item.Name,
                CompanyName = item.CompanyName,
                ContactPerson = item.ContactPerson,
                Phone = item.Phone,
                Email = item.Email,
                IsActive = item.IsActive
            });
        }
    }
}
