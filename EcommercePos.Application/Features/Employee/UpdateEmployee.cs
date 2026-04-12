using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Employee;

public static class UpdateEmployee
{
    public sealed record Command(
        Guid Id, string? FirstName, string? Phone, string? LastName, string? Gender,
        DateTime? DateOfBirth, string? Email, string? AddressLine1, string? City,
        DateTime? JoiningDate, DateTime? TerminationDate, string? Designation,
        string? Department, string? EmployeeType, decimal? Salary,
        string? BankName, string? BankAccountNumber, string? NationalId,
        string? EmergencyContactName, string? EmergencyContactPhone, bool? IsActive);

    public sealed record Response(Guid Id, string EmployeeCode, string FirstName, string? LastName,
        string? Phone, string? Email, string? Designation, string? Department, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Employees
                .Where(e => e.Id == command.Id && !e.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Employee '{command.Id}' was not found."));

            item.FirstName = command.FirstName ?? item.FirstName;
            item.LastName = command.LastName ?? item.LastName;
            item.Gender = command.Gender;
            item.DateOfBirth = command.DateOfBirth;
            item.Phone = command.Phone ?? item.Phone;
            item.Email = command.Email;
            item.AddressLine1 = command.AddressLine1 ?? item.AddressLine1;
            item.City = command.City ?? item.City;
            item.JoiningDate = command.JoiningDate ?? item.JoiningDate;
            item.TerminationDate = command.TerminationDate;
            item.Designation = command.Designation;
            item.Department = command.Department;
            item.EmployeeType = command.EmployeeType ?? item.EmployeeType;
            item.Salary = command.Salary ?? item.Salary;
            item.BankName = command.BankName;
            item.BankAccountNumber = command.BankAccountNumber;
            item.NationalId = command.NationalId;
            item.EmergencyContactName = command.EmergencyContactName;
            item.EmergencyContactPhone = command.EmergencyContactPhone;
            item.IsActive = command.IsActive ?? item.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(
                item.Id, item.EmployeeCode, item.FirstName, item.LastName,
                item.Phone, item.Email, item.Designation, item.Department, item.IsActive));
        }
    }
}
