using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Employee;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

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
            [FromBody] UpdateEmployee.Command body,
            UpdateEmployee.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { Id = id }, ct)).ToHttpResult())
            .AddEndpointFilter<ValidationFilter<UpdateEmployee.Command>>()
            .WithName("UpdateEmployee")
            .WithSummary("Update employee");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteEmployee.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteEmployee.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteEmployee")
            .WithSummary("Soft delete employee");

        group.MapPost("/{id:guid}/toggle-active", async (
            Guid id,
            ToggleEmployeeActive.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ToggleEmployeeActive.Command(id), ct)).ToHttpResult())
            .WithName("ToggleEmployeeActive")
            .WithSummary("Toggle employee active status");

        group.MapGet("/departments", async (
            GetDepartments.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetDepartments.Query(), ct)).ToHttpResult())
            .WithName("GetDepartments")
            .WithSummary("Get distinct employee departments");

        group.MapGet("/stats", async (
            GetEmployeeStats.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetEmployeeStats.Query(), ct)).ToHttpResult())
            .WithName("GetEmployeeStats")
            .WithSummary("Get employee statistics");
    }
}
