using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EcommercePos.Application.Features.Customer.Queries;

public static class GetCustomerById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public string CustomerCode { get; init; } = string.Empty;
        public string CustomerType { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string? AlternatePhone { get; init; }
        public string? Email { get; init; }
        public DateTime? DateOfBirth { get; init; }
        public string? Gender { get; init; }
        public string? CompanyName { get; init; }
        public string? TaxNumber { get; init; }
        public string? AddressLine1 { get; init; }
        public string? City { get; init; }
        public string Country { get; init; } = string.Empty;
        public decimal Balance { get; init; }
        public decimal? CreditLimit { get; init; }
        public int LoyaltyPoints { get; init; }
        public string? CustomerGroup { get; init; }
        public string? ReferralCode { get; init; }
        public DateTime RegistrationDate { get; init; }
        public DateTime? LastPurchaseDate { get; init; }
        public bool IsActive { get; init; }
        public string? Notes { get; init; }
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
            var item = await _context.Customers
                .Where(x => x.Id == query.Id && !x.IsDeleted)
                .AsNoTracking()
                .ProjectToType<Response>()
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Customer with id '{query.Id}' was not found."));
            }

            return Result<Response>.Success(item);
        }
    }
}