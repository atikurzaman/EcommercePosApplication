using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EcommercePos.Application.Features.Customer.Commands;

public static class UpdateCustomer
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
        Guid Id,
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
            var item = await _context.Customers
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Customer with id '{command.Id}' was not found."));
            }

            var exists = await _context.Customers
                .AnyAsync(x => x.CustomerCode == command.CustomerCode && x.Id != command.Id && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Another Customer with CustomerCode '{command.CustomerCode}' already exists."));
            }

            item.CustomerCode = command.CustomerCode;
            item.CustomerType = command.CustomerType;
            item.Phone = command.Phone;
            item.AlternatePhone = command.AlternatePhone;
            item.Email = command.Email;
            item.DateOfBirth = command.DateOfBirth;
            item.Gender = command.Gender;
            item.CompanyName = command.CompanyName;
            item.TaxNumber = command.TaxNumber;
            item.AddressLine1 = command.AddressLine1;
            item.City = command.City;
            item.Country = command.Country;
            item.CreditLimit = command.CreditLimit;
            item.CustomerGroup = command.CustomerGroup;
            item.IsActive = command.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}