using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Employee;

public static class GetEmployees
{
    public sealed record Query(
        int PageIndex = 0, int PageSize = 10, string? Search = null,
        string? Department = null, bool? IsActive = null);

    public sealed record Response(
        Guid Id, string EmployeeCode, string FirstName, string? LastName,
        string? Phone, string? Email, string? Designation, string? Department,
        DateTime? JoiningDate, string? EmployeeType, decimal? Salary, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Employees.Where(e => !e.IsDeleted).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(e =>
                    e.FirstName.Contains(query.Search) ||
                    (e.LastName != null && e.LastName.Contains(query.Search)) ||
                    (e.Phone != null && e.Phone.Contains(query.Search)) ||
                    e.EmployeeCode.Contains(query.Search));

            if (!string.IsNullOrWhiteSpace(query.Department))
                dbQuery = dbQuery.Where(e => e.Department == query.Department);

            if (query.IsActive.HasValue)
                dbQuery = dbQuery.Where(e => e.IsActive == query.IsActive.Value);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(e => e.FirstName)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(e => new Response(
                    e.Id, e.EmployeeCode, e.FirstName, e.LastName,
                    e.Phone, e.Email, e.Designation, e.Department,
                    e.JoiningDate, e.EmployeeType, e.Salary, e.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
