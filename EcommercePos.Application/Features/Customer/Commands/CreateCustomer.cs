using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using FluentValidation;
using Mapster;

namespace EcommercePos.Application.Features.Customer.Commands;

public static class CreateCustomer
{
    public sealed record Request
    {
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
        public string Country { get; init; } = "Bangladesh";
        public decimal? CreditLimit { get; init; }
        public string? CustomerGroup { get; init; }
        public bool IsActive { get; init; } = true;
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string CustomerCode { get; init; } = string.Empty;
        public string CompanyName { get; init; } = string.Empty;
        public string? ContactPerson { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public string? CustomerGroup { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(
        string CustomerCode,
        string CustomerType,
        string? Phone,
        string? AlternatePhone,
        string? Email,
        DateTime? DateOfBirth,
        string? Gender,
        string? CompanyName,
        string? TaxNumber,
        string? AddressLine1,
        string? City,
        string Country,
        decimal? CreditLimit,
        string? CustomerGroup,
        bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CustomerCode).NotEmpty();
            RuleFor(x => x.CustomerType).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var exists = await _context.Customers
                .AnyAsync(x => x.CustomerCode == command.CustomerCode && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Customer with CustomerCode '{command.CustomerCode}' already exists."));
            }

            var item = new Customers
            {
                CustomerCode = command.CustomerCode,
                CustomerType = command.CustomerType,
                Phone = command.Phone,
                AlternatePhone = command.AlternatePhone,
                Email = command.Email,
                DateOfBirth = command.DateOfBirth,
                Gender = command.Gender,
                CompanyName = command.CompanyName,
                TaxNumber = command.TaxNumber,
                AddressLine1 = command.AddressLine1,
                City = command.City,
                Country = command.Country,
                CreditLimit = command.CreditLimit,
                CustomerGroup = command.CustomerGroup,
                IsActive = command.IsActive,
                RegistrationDate = DateTime.UtcNow,
                Balance = 0,
                LoyaltyPoints = 0
            };

            _context.Customers.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}