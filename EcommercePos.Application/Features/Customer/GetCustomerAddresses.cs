using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Customer;

public static class GetCustomerAddresses
{
    public sealed record Query(Guid CustomerId);
    public sealed record Response(
        Guid Id, string? AddressType, string? Label, string? FullName,
        string? PhoneNumber, string? AddressLine1, string? AddressLine2,
        string? City, string? State, string? PostalCode, bool IsDefault);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;
        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var addresses = await _context.CustomerAddresses
                .Where(a => a.CustomerId == query.CustomerId && !a.IsDeleted)
                .OrderByDescending(a => a.IsDefault).ThenBy(a => a.CreatedAt)
                .Select(a => new Response(
                    a.Id, a.AddressType, a.Label, a.FullName, a.PhoneNumber,
                    a.AddressLine1, a.AddressLine2, a.City, a.State, a.PostalCode, a.IsDefault))
                .ToListAsync(ct);
            return Result<List<Response>>.Success(addresses);
        }
    }
}