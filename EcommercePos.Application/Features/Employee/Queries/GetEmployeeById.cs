using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EcommercePos.Application.Features.Employee.Queries;

public static class GetEmployeeById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
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
        public bool IsActive { get; init; }
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Employees
                .Where(x => x.Id == query.Id && !x.IsDeleted)
                .AsNoTracking()
                .ProjectToType<Response>()
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Employee with id '{query.Id}' was not found."));
            }

            return Result<Response>.Success(item);
        }
    }
}