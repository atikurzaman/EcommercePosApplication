using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Employee;

public static class GetEmployeeStats
{
    public sealed record Query();
    public sealed record Response(int TotalEmployees, int ActiveEmployees);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var totalEmployees = await _context.Employees.CountAsync(e => !e.IsDeleted, ct);
            var activeEmployees = await _context.Employees.CountAsync(e => !e.IsDeleted && e.IsActive, ct);
            return Result<Response>.Success(new Response(totalEmployees, activeEmployees));
        }
    }
}