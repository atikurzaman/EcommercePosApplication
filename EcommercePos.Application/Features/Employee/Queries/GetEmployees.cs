using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EcommercePos.Application.Features.Employee.Queries;

public static class GetEmployees
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

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

    public sealed record Query(int PageIndex, int PageSize, string? Search);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Employees
                .Where(x => !x.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(x => 
                    x.FirstName.Contains(query.Search) || 
                    x.LastName.Contains(query.Search) || 
                    x.EmployeeCode.Contains(query.Search) ||
                    x.Email.Contains(query.Search));
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .ProjectToType<Response>()
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}