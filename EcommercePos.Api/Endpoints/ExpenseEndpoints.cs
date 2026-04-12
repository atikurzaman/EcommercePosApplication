using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/expenses").WithTags("Expenses");

        group.MapGet("/", async (
            [AsParameters] GetExpenses.Request request,
            [FromServices] GetExpenses.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetExpenses")
        .WithSummary("Get paginated expenses with filters");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetExpenseById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetExpenseById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetExpenseById")
        .WithSummary("Get expense by id");

        group.MapPost("/", async (
            [FromBody] CreateExpense.Request request,
            [FromServices] CreateExpense.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/expenses/{result.Value?.Id}");
        })
        .WithName("CreateExpense")
        .WithSummary("Create a new expense");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateExpense.Request request,
            [FromServices] UpdateExpense.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateExpense.Command(
                id, request.ExpenseCategoryId, request.ExpenseDate,
                request.Description, request.Amount,
                request.MethodCode, request.ReceiptReference);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateExpense")
        .WithSummary("Update an existing expense");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteExpense.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteExpense.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteExpense")
        .WithSummary("Soft delete an expense");
    }
}
