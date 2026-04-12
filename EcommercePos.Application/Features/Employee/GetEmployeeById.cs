using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Employee;

public static class GetEmployeeById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string EmployeeCode, string FirstName, string? LastName,
        string? Gender, DateTime? DateOfBirth, string? Phone, string? Email,
        string? AddressLine1, string? City, DateTime? JoiningDate, DateTime? TerminationDate,
        string? Designation, string? Department, string? EmployeeType, decimal? Salary,
        string? BankName, string? BankAccountNumber, string? NationalId,
        string? EmergencyContactName, string? EmergencyContactPhone, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Employees
                .Where(e => e.Id == query.Id && !e.IsDeleted)
                .AsNoTracking()
                .Select(e => new Response(
                    e.Id, e.EmployeeCode, e.FirstName, e.LastName,
                    e.Gender, e.DateOfBirth, e.Phone, e.Email,
                    e.AddressLine1, e.City, e.JoiningDate, e.TerminationDate,
                    e.Designation, e.Department, e.EmployeeType, e.Salary,
                    e.BankName, e.BankAccountNumber, e.NationalId,
                    e.EmergencyContactName, e.EmergencyContactPhone, e.IsActive))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Employee '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
