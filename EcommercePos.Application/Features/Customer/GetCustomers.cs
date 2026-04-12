using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Customer;

public static class GetCustomers
{
    public sealed record Query(
        int PageIndex = 0, int PageSize = 20, string? Search = null, bool? IsActive = null);

    public sealed record Response(
        Guid Id, string CustomerCode, string CustomerType, string? Phone, string? Email,
        string? TierName, int LoyaltyPoints, bool IsActive,
        DateTime RegistrationDate, DateTime? LastPurchaseDate);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Customers
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(c =>
                    (c.Phone != null && c.Phone.Contains(query.Search)) ||
                    (c.Email != null && c.Email.Contains(query.Search)) ||
                    c.CustomerCode.Contains(query.Search) ||
                    (c.CompanyName != null && c.CompanyName.Contains(query.Search)));

            if (query.IsActive.HasValue)
                dbQuery = dbQuery.Where(c => c.IsActive == query.IsActive.Value);

            var totalCount = await dbQuery.CountAsync(ct);

            var customers = await dbQuery
                .OrderByDescending(c => c.RegistrationDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new
                {
                    c.Id, c.CustomerCode, c.CustomerType, c.Phone, c.Email,
                    c.LoyaltyPoints, c.IsActive, c.RegistrationDate, c.LastPurchaseDate,
                    TierName = c.CustomerProfiles != null
                        ? c.CustomerProfiles.TierCodeNavigation.DisplayName
                        : "Regular"
                })
                .ToListAsync(ct);

            var items = customers.Select(c => new Response(
                c.Id, c.CustomerCode, c.CustomerType, c.Phone, c.Email,
                c.TierName, c.LoyaltyPoints, c.IsActive, c.RegistrationDate, c.LastPurchaseDate))
                .ToList();

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
