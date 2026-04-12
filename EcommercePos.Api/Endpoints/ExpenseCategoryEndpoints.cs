using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.ExpenseCategory;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class ExpenseCategoryEndpoints
{
    public static void MapExpenseCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/expense-categories").WithTags("ExpenseCategories");

        group.MapGet("/", async (
            [AsParameters] GetExpenseCategories.Query request,
            GetExpenseCategories.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetExpenseCategories")
        .WithSummary("Get paginated expense categories");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetExpenseCategoryById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetExpenseCategoryById.Query(id), ct)).ToHttpResult())
        .WithName("GetExpenseCategoryById")
        .WithSummary("Get expense category by id");

        group.MapPost("/", async (
            [FromBody] CreateExpenseCategory.Command command,
            CreateExpenseCategory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/expense-categories"))
        .AddEndpointFilter<ValidationFilter<CreateExpenseCategory.Command>>()
        .WithName("CreateExpenseCategory")
        .WithSummary("Create a new expense category");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateExpenseCategory.Command command,
            UpdateExpenseCategory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateExpenseCategory.Command(
                id, command.Name, command.Description, command.IsActive), ct)).ToHttpResult())
        .AddEndpointFilter<ValidationFilter<UpdateExpenseCategory.Command>>()
        .WithName("UpdateExpenseCategory")
        .WithSummary("Update an existing expense category");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteExpenseCategory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteExpenseCategory.Command(id), ct)).ToNoContentResult())
        .WithName("DeleteExpenseCategory")
        .WithSummary("Soft delete an expense category");
    }
}
