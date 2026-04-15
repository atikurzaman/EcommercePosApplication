using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Supplier;

public static class CreateSupplier
{
    public sealed record Command(
        string Name, string? Phone, string? CompanyName, string? ContactPerson,
        string? AlternatePhone, string? Email, string? AddressLine1, string? AddressLine2,
        string? City, string? State, string? PostalCode, string? Country,
        string? SupplierType, string? TaxRegistrationNo, string? PaymentTerms,
        int? LeadTimeDays, string? Notes);

    public sealed record Response(Guid Id, string SupplierCode, string Name, string? CompanyName,
        string? ContactPerson, string? Phone, string? Email, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(command.Phone))
            {
                var exists = await _context.Suppliers
                    .AnyAsync(s => s.Phone == command.Phone && !s.IsDeleted, ct);
                if (exists)
                    return Result<Response>.Failure(Error.Conflict("Supplier with this phone number already exists."));
            }

            var supplierCode = $"SUP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
            var item = new Suppliers
            {
                Id = Guid.NewGuid(),
                SupplierCode = supplierCode,
                Name = command.Name,
                CompanyName = command.CompanyName,
                ContactPerson = command.ContactPerson,
                Phone = command.Phone,
                AlternatePhone = command.AlternatePhone,
                Email = command.Email,
                AddressLine1 = command.AddressLine1,
                AddressLine2 = command.AddressLine2,
                City = command.City ?? "Dhaka",
                State = command.State,
                PostalCode = command.PostalCode,
                Country = command.Country ?? "Bangladesh",
                SupplierType = command.SupplierType ?? "MANUFACTURER",
                TaxRegistrationNo = command.TaxRegistrationNo,
                PaymentTerms = command.PaymentTerms,
                LeadTimeDays = command.LeadTimeDays,
                Notes = command.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Suppliers.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                item.Id, item.SupplierCode, item.Name, item.CompanyName,
                item.ContactPerson, item.Phone, item.Email, item.IsActive));
        }
    }
}
