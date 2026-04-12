using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Supplier;

public static class GetSupplierById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string SupplierCode, string Name, string? CompanyName,
        string? ContactPerson, string? Phone, string? AlternatePhone, string? Email,
        string? AddressLine1, string? AddressLine2, string? City, string? State,
        string? PostalCode, string? Country, string? SupplierType,
        string? TaxRegistrationNo, string? PaymentTerms, int? LeadTimeDays,
        string? Notes, bool IsActive, DateTime CreatedAt);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Suppliers
                .Where(s => s.Id == query.Id && !s.IsDeleted)
                .AsNoTracking()
                .Select(s => new Response(
                    s.Id, s.SupplierCode, s.Name, s.CompanyName, s.ContactPerson,
                    s.Phone, s.AlternatePhone, s.Email,
                    s.AddressLine1, s.AddressLine2, s.City, s.State,
                    s.PostalCode, s.Country, s.SupplierType,
                    s.TaxRegistrationNo, s.PaymentTerms, s.LeadTimeDays,
                    s.Notes, s.IsActive, s.CreatedAt))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Supplier '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
