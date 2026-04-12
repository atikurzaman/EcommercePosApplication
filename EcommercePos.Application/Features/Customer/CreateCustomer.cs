using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Customer;

public static class CreateCustomer
{
    public sealed record Command(
        string? Phone, string? CustomerType, string? AlternatePhone, string? Email,
        DateTime? DateOfBirth, string? Gender, string? CompanyName, string? TaxNumber,
        string? AddressLine1, string? City, string? Country, decimal? CreditLimit);

    public sealed record Response(Guid Id, string CustomerCode, string? Phone, string? Email);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(command.Phone))
            {
                var exists = await _context.Customers
                    .AnyAsync(c => c.Phone == command.Phone && !c.IsDeleted, ct);
                if (exists)
                    return Result<Response>.Failure(Error.Conflict("Customer with this phone number already exists."));
            }

            var customerCode = $"CUST-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
            var item = new Customers
            {
                Id = Guid.NewGuid(),
                CustomerCode = customerCode,
                CustomerType = command.CustomerType ?? "RETAIL",
                Phone = command.Phone,
                AlternatePhone = command.AlternatePhone,
                Email = command.Email,
                DateOfBirth = command.DateOfBirth,
                Gender = command.Gender,
                CompanyName = command.CompanyName,
                TaxNumber = command.TaxNumber,
                AddressLine1 = command.AddressLine1,
                City = command.City ?? "Dhaka",
                Country = command.Country ?? "Bangladesh",
                CreditLimit = command.CreditLimit ?? 0,
                IsActive = true,
                RegistrationDate = DateTime.UtcNow,
                Balance = 0,
                LoyaltyPoints = 0,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Customers.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(item.Id, item.CustomerCode, item.Phone, item.Email));
        }
    }
}
