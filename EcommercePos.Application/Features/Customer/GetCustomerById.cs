using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Customer;

public static class GetCustomerById
{
    public sealed record Query(Guid Id);

    public sealed record AddressResponse(
        Guid Id, string AddressType, string? Label, string FullName, string PhoneNumber,
        string AddressLine1, string? AddressLine2, string City, string? State,
        string? PostalCode, bool IsDefault);

    public sealed record TierResponse(
        string TierCode, string DisplayName, decimal DiscountPct, decimal PointsMultiplier);

    public sealed record OrderSummary(
        Guid Id, string OrderNumber, string Status, decimal TotalAmount, DateTime OrderDate);

    public sealed record Response(
        Guid Id, string CustomerCode, string CustomerType,
        string? Phone, string? AlternatePhone, string? Email,
        DateTime? DateOfBirth, string? Gender, string? CompanyName, string? TaxNumber,
        string? AddressLine1, string? City, string? Country,
        decimal Balance, decimal? CreditLimit, int LoyaltyPoints,
        DateTime RegistrationDate, DateTime? LastPurchaseDate, bool IsActive,
        TierResponse? Tier, List<AddressResponse> Addresses, List<OrderSummary> RecentOrders);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerProfiles).ThenInclude(p => p!.TierCodeNavigation)
                .Include(c => c.CustomerAddresses)
                .Include(c => c.Orders).ThenInclude(o => o.StatusCodeNavigation)
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (customer is null)
                return Result<Response>.Failure(Error.NotFound($"Customer '{query.Id}' was not found."));

            var tier = customer.CustomerProfiles is not null
                ? new TierResponse(
                    customer.CustomerProfiles.TierCode,
                    customer.CustomerProfiles.TierCodeNavigation.DisplayName,
                    customer.CustomerProfiles.TierCodeNavigation.DiscountPct,
                    customer.CustomerProfiles.TierCodeNavigation.PointsMultiplier)
                : null;

            var addresses = customer.CustomerAddresses
                .Where(a => !a.IsDeleted)
                .Select(a => new AddressResponse(
                    a.Id, a.AddressType, a.Label, a.FullName, a.PhoneNumber,
                    a.AddressLine1, a.AddressLine2, a.City, a.State, a.PostalCode, a.IsDefault))
                .ToList();

            var recentOrders = customer.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new OrderSummary(
                    o.Id, o.OrderNumber, o.StatusCodeNavigation.DisplayName,
                    o.TotalAmount, o.OrderDate))
                .ToList();

            return Result<Response>.Success(new Response(
                customer.Id, customer.CustomerCode, customer.CustomerType,
                customer.Phone, customer.AlternatePhone, customer.Email,
                customer.DateOfBirth, customer.Gender, customer.CompanyName, customer.TaxNumber,
                customer.AddressLine1, customer.City, customer.Country,
                customer.Balance, customer.CreditLimit, customer.LoyaltyPoints,
                customer.RegistrationDate, customer.LastPurchaseDate, customer.IsActive,
                tier, addresses, recentOrders));
        }
    }
}
