using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EcommercePos.Application.Features.Employee.Commands;

public static class CreateEmployee
{
    public sealed record Request
    {
        public string EmployeeCode { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string? LastName { get; init; }
        public string? Gender { get; init; }
        public DateTime? DateOfBirth { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public string? AddressLine1 { get; init; }
        public string? City { get; init; }
        public DateTime? JoiningDate { get; init; }
        public DateTime? TerminationDate { get; init; }
        public string? Designation { get; init; }
        public string? Department { get; init; }
        public string? EmployeeType { get; init; }
        public decimal? Salary { get; init; }
        public string? BankName { get; init; }
        public string? BankAccountNumber { get; init; }
        public string? NationalId { get; init; }
        public string? EmergencyContactName { get; init; }
        public string? EmergencyContactPhone { get; init; }
        public bool IsActive { get; init; } = true;
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string EmployeeCode { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string? LastName { get; init; }
        public string? Phone { get; init; }
        public string? Email { get; init; }
        public string? Designation { get; init; }
        public string? Department { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(
        string EmployeeCode, string FirstName, string? LastName, string? Gender, DateTime? DateOfBirth,
        string? Phone, string? Email, string? AddressLine1, string? City, DateTime? JoiningDate,
        DateTime? TerminationDate, string? Designation, string? Department, string? EmployeeType,
        decimal? Salary, string? BankName, string? BankAccountNumber, string? NationalId,
        string? EmergencyContactName, string? EmergencyContactPhone, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.EmployeeCode).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
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
            var exists = await _context.Employees
                .AnyAsync(x => x.EmployeeCode == command.EmployeeCode && !x.IsDeleted, ct);

            if (exists)
            {
                return Result<Response>.Failure(Error.Conflict($"Employee with EmployeeCode '{command.EmployeeCode}' already exists."));
            }

            var item = new Employees
            {
                EmployeeCode = command.EmployeeCode,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Gender = command.Gender,
                DateOfBirth = command.DateOfBirth,
                Phone = command.Phone,
                Email = command.Email,
                AddressLine1 = command.AddressLine1,
                City = command.City,
                JoiningDate = command.JoiningDate,
                TerminationDate = command.TerminationDate,
                Designation = command.Designation,
                Department = command.Department,
                EmployeeType = command.EmployeeType,
                Salary = command.Salary,
                BankName = command.BankName,
                BankAccountNumber = command.BankAccountNumber,
                NationalId = command.NationalId,
                EmergencyContactName = command.EmergencyContactName,
                EmergencyContactPhone = command.EmergencyContactPhone,
                IsActive = command.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}