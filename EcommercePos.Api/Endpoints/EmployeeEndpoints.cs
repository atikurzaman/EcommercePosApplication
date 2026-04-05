using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Employee.Queries;
using EcommercePos.Application.Features.Employee.Commands;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees").WithTags("Employees");

        group.MapGet("/", async (
            [AsParameters] GetEmployees.Request request,
            [FromServices] GetEmployees.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetEmployees.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetEmployees")
        .WithSummary("Get paginated employees");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetEmployeeById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetEmployeeById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetEmployeeById")
        .WithSummary("Get employee by id");

        group.MapPost("/", async (
            [FromBody] CreateEmployee.Request request,
            [FromServices] CreateEmployee.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateEmployee.Command(
                request.EmployeeCode, request.FirstName, request.LastName,
                request.Gender, request.DateOfBirth, request.Phone, request.Email,
                request.AddressLine1, request.City, request.JoiningDate,
                request.TerminationDate, request.Designation, request.Department,
                request.EmployeeType, request.Salary, request.BankName,
                request.BankAccountNumber, request.NationalId, request.EmergencyContactName,
                request.EmergencyContactPhone, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/employees/{command.EmployeeCode}");
        })
        .WithName("CreateEmployee")
        .WithSummary("Create a new employee");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateEmployee.Request request,
            [FromServices] UpdateEmployee.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateEmployee.Command(
                id, request.EmployeeCode, request.FirstName, request.LastName,
                request.Gender, request.DateOfBirth, request.Phone, request.Email,
                request.AddressLine1, request.City, request.JoiningDate,
                request.TerminationDate, request.Designation, request.Department,
                request.EmployeeType, request.Salary, request.BankName,
                request.BankAccountNumber, request.NationalId, request.EmergencyContactName,
                request.EmergencyContactPhone, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateEmployee")
        .WithSummary("Update an existing employee");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteEmployee.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteEmployee.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteEmployee")
        .WithSummary("Soft delete an employee");
    }
}
