using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Employee;

public static class GetDepartments
{
    public sealed record Query();
    public sealed record Response(string Department);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var departments = await _context.Employees
                .Where(e => !e.IsDeleted && e.Department != null)
                .Select(e => e.Department!)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(ct);
            return Result<List<Response>>.Success(departments.Select(d => new Response(d)).ToList());
        }
    }
}