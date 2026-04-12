using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees").WithTags("Employees");

        group.MapGet("/", async (
            [AsParameters] GetEmployeesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Employees
                .Where(e => !e.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(e => 
                    e.FirstName.Contains(request.Search) || 
                    e.LastName.Contains(request.Search) || 
                    e.Phone.Contains(request.Search) ||
                    e.EmployeeCode.Contains(request.Search));

            if (!string.IsNullOrWhiteSpace(request.Department))
                query = query.Where(e => e.Department == request.Department);

            if (request.IsActive.HasValue)
                query = query.Where(e => e.IsActive == request.IsActive);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(e => e.FirstName)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new EmployeeListResponse(
                    e.Id, e.EmployeeCode, e.FirstName ?? "", e.LastName ?? "", e.Phone ?? "", e.Email,
                    e.Designation, e.Department, e.Department,
                    e.JoiningDate, e.EmployeeType, e.Salary, e.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetEmployees")
        .WithSummary("Get paginated employees");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var employee = await context.Employees
                .Where(e => e.Id == id && !e.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (employee == null)
                return Results.NotFound(new { error = "Employee not found" });

            var response = new EmployeeDetailResponse(
                employee.Id, employee.EmployeeCode, employee.FirstName, employee.LastName,
                employee.Gender, employee.DateOfBirth, employee.Phone ?? "", employee.Email,
                employee.AddressLine1, employee.City, employee.JoiningDate, employee.TerminationDate,
                employee.Designation, employee.Department, employee.Department,
                employee.EmployeeType, employee.Salary, employee.BankName, employee.BankAccountNumber,
                employee.NationalId, employee.EmergencyContactName, employee.EmergencyContactPhone,
                employee.IsActive, employee.CreatedAt);

            return Results.Ok(new { data = response });
        })
        .WithName("GetEmployeeById")
        .WithSummary("Get employee with details");

        group.MapPost("/", async (CreateEmployeeRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.Employees.AnyAsync(e => e.Phone == request.Phone && !e.IsDeleted, ct);
            if (exists)
                return Results.Conflict(new { error = "Employee with this phone already exists" });

            var employee = new Employees
            {
                Id = Guid.NewGuid(),
                EmployeeCode = $"EMP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
                FirstName = request.FirstName,
                LastName = request.LastName ?? "",
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                Phone = request.Phone,
                Email = request.Email,
                AddressLine1 = request.AddressLine1,
                City = request.City ?? "Dhaka",
                JoiningDate = request.JoiningDate ?? DateTime.UtcNow,
                TerminationDate = request.TerminationDate,
                Designation = request.Designation,
                Department = request.Department,
                EmployeeType = request.EmployeeType ?? "FULL_TIME",
                Salary = request.Salary,
                BankName = request.BankName,
                BankAccountNumber = request.BankAccountNumber,
                NationalId = request.NationalId,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Employees.Add(employee);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/employees/{employee.Id}", new { data = new { employee.Id, employee.EmployeeCode, employee.FirstName } });
        })
        .WithName("CreateEmployee")
        .WithSummary("Create a new employee");

        group.MapPut("/{id:guid}", async (Guid id, UpdateEmployeeRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var employee = await context.Employees.FindAsync(new object[] { id }, ct);
            if (employee == null || employee.IsDeleted)
                return Results.NotFound(new { error = "Employee not found" });

            employee.FirstName = request.FirstName ?? employee.FirstName;
            employee.LastName = request.LastName ?? employee.LastName;
            employee.Gender = request.Gender;
            employee.DateOfBirth = request.DateOfBirth;
            employee.Phone = request.Phone ?? employee.Phone;
            employee.Email = request.Email;
            employee.AddressLine1 = request.AddressLine1 ?? employee.AddressLine1;
            employee.City = request.City ?? employee.City;
            employee.JoiningDate = request.JoiningDate ?? employee.JoiningDate;
            employee.TerminationDate = request.TerminationDate;
            employee.Designation = request.Designation;
            employee.Department = request.Department;
            employee.EmployeeType = request.EmployeeType ?? employee.EmployeeType;
            employee.Salary = request.Salary ?? employee.Salary;
            employee.BankName = request.BankName;
            employee.BankAccountNumber = request.BankAccountNumber;
            employee.NationalId = request.NationalId;
            employee.EmergencyContactName = request.EmergencyContactName;
            employee.EmergencyContactPhone = request.EmergencyContactPhone;
            employee.IsActive = request.IsActive ?? employee.IsActive;
            employee.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { employee.Id } });
        })
        .WithName("UpdateEmployee")
        .WithSummary("Update employee");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var employee = await context.Employees.FindAsync(new object[] { id }, ct);
            if (employee == null || employee.IsDeleted)
                return Results.NotFound(new { error = "Employee not found" });

            employee.IsDeleted = true;
            employee.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteEmployee")
        .WithSummary("Soft delete employee");

        group.MapPost("/{id:guid}/toggle-active", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var employee = await context.Employees.FindAsync(new object[] { id }, ct);
            if (employee == null || employee.IsDeleted)
                return Results.NotFound(new { error = "Employee not found" });

            employee.IsActive = !employee.IsActive;
            employee.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { employee.Id, employee.IsActive } });
        })
        .WithName("ToggleEmployeeActive")
        .WithSummary("Toggle employee active status");

        group.MapGet("/departments", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var departments = await context.Employees
                .Select(e => e.Department)
                .Where(d => d != null)
                .Distinct()
                .ToListAsync(ct);

            return Results.Ok(new { data = departments });
        })
        .WithName("GetDepartments")
        .WithSummary("Get employee departments");

        group.MapGet("/stats", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var stats = new
            {
                TotalEmployees = await context.Employees.Where(e => !e.IsDeleted).CountAsync(ct),
                ActiveEmployees = await context.Employees.Where(e => !e.IsDeleted && e.IsActive).CountAsync(ct),
            };

            return Results.Ok(new { data = stats });
        })
        .WithName("GetEmployeeStats")
        .WithSummary("Get employee statistics");
    }
}

public record GetEmployeesRequest(
    int PageIndex = 0, int PageSize = 20, string? Search = null,
    string? Department = null, bool? IsActive = null);

public record EmployeeListResponse(
    Guid Id, string EmployeeCode, string FirstName, string LastName,
    string Phone, string? Email, string? Designation, string? Department, string? DepartmentName,
    DateTime? JoiningDate, string? EmployeeType, decimal? Salary, bool IsActive);

public record EmployeeDetailResponse(
    Guid Id, string EmployeeCode, string FirstName, string LastName,
    string? Gender, DateTime? DateOfBirth, string Phone, string? Email,
    string? AddressLine1, string? City, DateTime? JoiningDate, DateTime? TerminationDate,
    string? Designation, string? Department, string? DepartmentName,
    string? EmployeeType, decimal? Salary, string? BankName, string? BankAccountNumber,
    string? NationalId, string? EmergencyContactName, string? EmergencyContactPhone,
    bool IsActive, DateTime CreatedAt);

public record CreateEmployeeRequest(
    string FirstName, string Phone, string? LastName, string? Gender,
    DateTime? DateOfBirth, string? Email, string? AddressLine1, string? City,
    DateTime? JoiningDate, DateTime? TerminationDate, string? Designation,
    string? Department, string? EmployeeType, decimal? Salary, string? BankName,
    string? BankAccountNumber, string? NationalId, string? EmergencyContactName,
    string? EmergencyContactPhone);

public record UpdateEmployeeRequest(
    string? FirstName, string? Phone, string? LastName, string? Gender,
    DateTime? DateOfBirth, string? Email, string? AddressLine1, string? City,
    DateTime? JoiningDate, DateTime? TerminationDate, string? Designation,
    string? Department, string? EmployeeType, decimal? Salary, string? BankName,
    string? BankAccountNumber, string? NationalId, string? EmergencyContactName,
    string? EmergencyContactPhone, bool? IsActive);