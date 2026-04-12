using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Employee;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees").WithTags("Employees");

        group.MapGet("/", async (
            [AsParameters] GetEmployees.Query query,
            GetEmployees.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetEmployees")
            .WithSummary("Get paginated employees");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetEmployeeById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetEmployeeById.Query(id), ct)).ToHttpResult())
            .WithName("GetEmployeeById")
            .WithSummary("Get employee by id");

        group.MapPost("/", async (
            [FromBody] CreateEmployee.Command command,
            CreateEmployee.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/employees"))
            .AddEndpointFilter<ValidationFilter<CreateEmployee.Command>>()
            .WithName("CreateEmployee")
            .WithSummary("Create a new employee");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateEmployeeBody body,
            UpdateEmployee.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateEmployee.Command(
                id, body.FirstName, body.Phone, body.LastName, body.Gender,
                body.DateOfBirth, body.Email, body.AddressLine1, body.City,
                body.JoiningDate, body.TerminationDate, body.Designation,
                body.Department, body.EmployeeType, body.Salary,
                body.BankName, body.BankAccountNumber, body.NationalId,
                body.EmergencyContactName, body.EmergencyContactPhone, body.IsActive), ct)).ToHttpResult())
            .AddEndpointFilter<ValidationFilter<UpdateEmployeeBody>>()
            .WithName("UpdateEmployee")
            .WithSummary("Update employee");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteEmployee.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteEmployee.Command(id), ct)).ToNoContentResult())
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
                .Where(e => !e.IsDeleted && e.Department != null)
                .Select(e => e.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(ct);

            return Results.Ok(new { data = departments });
        })
        .WithName("GetDepartments")
        .WithSummary("Get distinct employee departments");

        group.MapGet("/stats", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var stats = new
            {
                TotalEmployees = await context.Employees.CountAsync(e => !e.IsDeleted, ct),
                ActiveEmployees = await context.Employees.CountAsync(e => !e.IsDeleted && e.IsActive, ct)
            };

            return Results.Ok(new { data = stats });
        })
        .WithName("GetEmployeeStats")
        .WithSummary("Get employee statistics");
    }
}

public record UpdateEmployeeBody(
    string? FirstName, string? Phone, string? LastName, string? Gender,
    DateTime? DateOfBirth, string? Email, string? AddressLine1, string? City,
    DateTime? JoiningDate, DateTime? TerminationDate, string? Designation,
    string? Department, string? EmployeeType, decimal? Salary, string? BankName,
    string? BankAccountNumber, string? NationalId, string? EmergencyContactName,
    string? EmergencyContactPhone, bool? IsActive);
