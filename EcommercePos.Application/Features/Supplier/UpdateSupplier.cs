using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Supplier;

public static class UpdateSupplier
{
    public sealed record Command(
        Guid Id, string? Name, string? Phone, string? CompanyName, string? ContactPerson,
        string? AlternatePhone, string? Email, string? AddressLine1, string? AddressLine2,
        string? City, string? State, string? PostalCode, string? Country,
        string? SupplierType, string? TaxRegistrationNo, string? PaymentTerms,
        int? LeadTimeDays, string? Notes, bool? IsActive);

    public sealed record Response(Guid Id, string SupplierCode, string Name, string? Phone,
        string? Email, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Suppliers
                .Where(s => s.Id == command.Id && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Supplier '{command.Id}' was not found."));

            item.Name = command.Name ?? item.Name;
            item.CompanyName = command.CompanyName;
            item.ContactPerson = command.ContactPerson;
            item.Phone = command.Phone ?? item.Phone;
            item.AlternatePhone = command.AlternatePhone;
            item.Email = command.Email;
            item.AddressLine1 = command.AddressLine1 ?? item.AddressLine1;
            item.AddressLine2 = command.AddressLine2;
            item.City = command.City ?? item.City;
            item.State = command.State;
            item.PostalCode = command.PostalCode;
            item.Country = command.Country ?? item.Country;
            item.SupplierType = command.SupplierType ?? item.SupplierType;
            item.TaxRegistrationNo = command.TaxRegistrationNo;
            item.PaymentTerms = command.PaymentTerms;
            item.LeadTimeDays = command.LeadTimeDays;
            item.Notes = command.Notes;
            item.IsActive = command.IsActive ?? item.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(
                item.Id, item.SupplierCode, item.Name, item.Phone, item.Email, item.IsActive));
        }
    }
}
