using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Customer;

public static class ToggleCustomerActive
{
    public sealed record Command(Guid CustomerId);
    public sealed record Response(Guid Id, bool IsActive);
    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var customer = await _context.Customers.FindAsync(new object[] { command.CustomerId }, ct);
            if (customer == null || customer.IsDeleted)
                return Result<Response>.Failure(Error.NotFound("Customer not found"));
            customer.IsActive = !customer.IsActive;
            customer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(customer.Id, customer.IsActive));
        }
    }
}