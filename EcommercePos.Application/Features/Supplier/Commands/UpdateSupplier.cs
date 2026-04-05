using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using FluentValidation;

namespace EcommercePos.Application.Features.Supplier.Commands;

public static class UpdateSupplier
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
        public bool IsActive { get; init; }
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
        Guid Id, string SupplierCode, string Name, string? CompanyName, string? ContactPerson,
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
            var item = await _context.Suppliers
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Supplier with id '{command.Id}' was not found."));
            }

            var exists = await _context.Suppliers
                .AnyAsync(x => x.SupplierCode == command.SupplierCode && x.Id != command.Id && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Another Supplier with SupplierCode '{command.SupplierCode}' already exists."));
            }

            item.SupplierCode = command.SupplierCode;
            item.Name = command.Name;
            item.CompanyName = command.CompanyName;
            item.ContactPerson = command.ContactPerson;
            item.Phone = command.Phone;
            item.AlternatePhone = command.AlternatePhone;
            item.Email = command.Email;
            item.AddressLine1 = command.AddressLine1;
            item.AddressLine2 = command.AddressLine2;
            item.City = command.City;
            item.State = command.State;
            item.PostalCode = command.PostalCode;
            item.Country = command.Country;
            item.SupplierType = command.SupplierType;
            item.TaxRegistrationNo = command.TaxRegistrationNo;
            item.PaymentTerms = command.PaymentTerms;
            item.LeadTimeDays = command.LeadTimeDays;
            item.Notes = command.Notes;
            item.IsActive = command.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

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
