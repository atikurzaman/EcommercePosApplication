using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Category.Queries;
using EcommercePos.Application.Features.Category.Commands;
using EcommercePos.Api.Extensions;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", async (
            [AsParameters] GetCategories.Request request,
            [FromServices] GetCategories.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetCategories.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetCategories")
        .WithSummary("Get paginated categories");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetCategoryById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCategoryById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCategoryById")
        .WithSummary("Get category by id");

        group.MapPost("/", async (
            [FromBody] CreateCategory.Request request,
            [FromServices] CreateCategory.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateCategory.Command(
                request.Name, request.Description, request.ImageUrl,
                request.ParentCategoryId, request.DisplayOrder, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/categories/{command.Name}");
        })
        .WithName("CreateCategory")
        .WithSummary("Create a new category");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateCategory.Request request,
            [FromServices] UpdateCategory.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateCategory.Command(
                id, request.Name, request.Description, request.ImageUrl,
                request.ParentCategoryId, request.DisplayOrder, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateCategory")
        .WithSummary("Update an existing category");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteCategory.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteCategory.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteCategory")
        .WithSummary("Soft delete a category");
    }
}