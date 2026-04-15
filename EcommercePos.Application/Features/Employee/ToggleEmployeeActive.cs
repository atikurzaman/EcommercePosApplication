using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Employee;

public static class ToggleEmployeeActive
{
    public sealed record Command(Guid EmployeeId);
    public sealed record Response(Guid Id, bool IsActive);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var employee = await _context.Employees.FindAsync(new object[] { command.EmployeeId }, ct);
            if (employee == null || employee.IsDeleted)
                return Result<Response>.Failure(Error.NotFound("Employee not found"));
            employee.IsActive = !employee.IsActive;
            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(employee.Id, employee.IsActive));
        }
    }
}