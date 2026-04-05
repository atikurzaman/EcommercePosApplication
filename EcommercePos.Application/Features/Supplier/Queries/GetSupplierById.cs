using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Supplier.Queries;

public static class GetSupplierById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
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
        public decimal Balance { get; init; }
        public string? Notes { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Suppliers
                .Where(x => x.Id == query.Id && !x.IsDeleted)
                .AsNoTracking()
                .Select(x => new Response
                {
                    Id = x.Id,
                    SupplierCode = x.SupplierCode,
                    Name = x.Name,
                    CompanyName = x.CompanyName,
                    ContactPerson = x.ContactPerson,
                    Phone = x.Phone,
                    AlternatePhone = x.AlternatePhone,
                    Email = x.Email,
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    City = x.City,
                    State = x.State,
                    PostalCode = x.PostalCode,
                    Country = x.Country,
                    SupplierType = x.SupplierType,
                    TaxRegistrationNo = x.TaxRegistrationNo,
                    PaymentTerms = x.PaymentTerms,
                    LeadTimeDays = x.LeadTimeDays,
                    Balance = x.Balance,
                    Notes = x.Notes,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Supplier with id '{query.Id}' was not found."));
            }

            return Result<Response>.Success(item);
        }
    }
}
