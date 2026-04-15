using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Customer;

public static class UpdateCustomer
{
    public sealed record Command(
        Guid Id, string? Phone, string? AlternatePhone, string? Email,
        DateTime? DateOfBirth, string? Gender, string? CompanyName, string? TaxNumber,
        string? AddressLine1, string? City, string? Country, decimal? CreditLimit, bool IsActive);

    public sealed record Response(Guid Id, string CustomerCode, string? Phone, string? Email, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Customers
                .Where(c => c.Id == command.Id && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Customer '{command.Id}' was not found."));

            item.Phone = command.Phone ?? item.Phone;
            item.AlternatePhone = command.AlternatePhone;
            item.Email = command.Email;
            item.DateOfBirth = command.DateOfBirth;
            item.Gender = command.Gender;
            item.CompanyName = command.CompanyName;
            item.TaxNumber = command.TaxNumber;
            item.AddressLine1 = command.AddressLine1 ?? item.AddressLine1;
            item.City = command.City ?? item.City;
            item.Country = command.Country ?? item.Country;
            item.CreditLimit = command.CreditLimit ?? item.CreditLimit;
            item.IsActive = command.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(
                item.Id, item.CustomerCode, item.Phone, item.Email, item.IsActive));
        }
    }
}
