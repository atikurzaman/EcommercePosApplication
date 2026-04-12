using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Employee;

public static class CreateEmployee
{
    public sealed record Command(
        string FirstName, string? Phone,
        string? LastName, string? Gender, DateTime? DateOfBirth, string? Email,
        string? AddressLine1, string? City, DateTime? JoiningDate, DateTime? TerminationDate,
        string? Designation, string? Department, string? EmployeeType, decimal? Salary,
        string? BankName, string? BankAccountNumber, string? NationalId,
        string? EmergencyContactName, string? EmergencyContactPhone);

    public sealed record Response(Guid Id, string EmployeeCode, string FirstName, string? LastName,
        string? Phone, string? Email, string? Designation, string? Department, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
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
                var exists = await _context.Employees
                    .AnyAsync(e => e.Phone == command.Phone && !e.IsDeleted, ct);
                if (exists)
                    return Result<Response>.Failure(Error.Conflict("Employee with this phone number already exists."));
            }

            var empCode = $"EMP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
            var item = new Employees
            {
                Id = Guid.NewGuid(),
                EmployeeCode = empCode,
                FirstName = command.FirstName,
                LastName = command.LastName ?? "",
                Gender = command.Gender,
                DateOfBirth = command.DateOfBirth,
                Phone = command.Phone,
                Email = command.Email,
                AddressLine1 = command.AddressLine1,
                City = command.City ?? "Dhaka",
                JoiningDate = command.JoiningDate ?? DateTime.UtcNow,
                TerminationDate = command.TerminationDate,
                Designation = command.Designation,
                Department = command.Department,
                EmployeeType = command.EmployeeType ?? "FULL_TIME",
                Salary = command.Salary,
                BankName = command.BankName,
                BankAccountNumber = command.BankAccountNumber,
                NationalId = command.NationalId,
                EmergencyContactName = command.EmergencyContactName,
                EmergencyContactPhone = command.EmergencyContactPhone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Employees.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                item.Id, item.EmployeeCode, item.FirstName, item.LastName,
                item.Phone, item.Email, item.Designation, item.Department, item.IsActive));
        }
    }
}
