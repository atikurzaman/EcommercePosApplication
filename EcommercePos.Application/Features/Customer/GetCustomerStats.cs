using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Customer;

public static class GetCustomerStats
{
    public sealed record Query();
    public sealed record Response(
        int TotalCustomers,
        int ActiveCustomers,
        int NewCustomersToday,
        long TotalLoyaltyPoints);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;
            var totalCustomers = await _context.Customers.CountAsync(c => !c.IsDeleted, ct);
            var activeCustomers = await _context.Customers.CountAsync(c => !c.IsDeleted && c.IsActive, ct);
            var newCustomersToday = await _context.Customers.CountAsync(c => !c.IsDeleted && c.RegistrationDate >= today, ct);
            var totalLoyaltyPoints = await _context.Customers.Where(c => !c.IsDeleted).SumAsync(c => c.LoyaltyPoints, ct);
            return Result<Response>.Success(new Response(totalCustomers, activeCustomers, newCustomersToday, totalLoyaltyPoints));
        }
    }
}