using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Category;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", async (
            [AsParameters] GetCategories.Query query,
            GetCategories.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetCategories")
            .WithSummary("Get paginated categories");

        group.MapGet("/tree", async (
            GetCategoryTree.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetCategoryTree.Query(), ct)).ToHttpResult())
            .WithName("GetCategoryTree")
            .WithSummary("Get hierarchical category tree");

        group.MapGet("/flat", async (
            GetCategoryFlat.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetCategoryFlat.Query(), ct)).ToHttpResult())
            .WithName("GetCategoryFlat")
            .WithSummary("Get flat category list for dropdowns");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetCategoryById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetCategoryById.Query(id), ct)).ToHttpResult())
            .WithName("GetCategoryById")
            .WithSummary("Get category by id");

        group.MapPost("/", async (
            [FromBody] CreateCategory.Command command,
            CreateCategory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/categories"))
            .AddEndpointFilter<ValidationFilter<CreateCategory.Command>>()
            .WithName("CreateCategory")
            .WithSummary("Create a new category");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateCategory.Command body,
            UpdateCategory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { Id = id }, ct)).ToHttpResult())
            .AddEndpointFilter<ValidationFilter<UpdateCategory.Command>>()
            .WithName("UpdateCategory")
            .WithSummary("Update an existing category");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteCategory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteCategory.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteCategory")
            .WithSummary("Soft delete a category");

        group.MapPatch("/{id:guid}/toggle", async (
            Guid id,
            ToggleCategoryActive.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ToggleCategoryActive.Command(id), ct)).ToHttpResult())
            .WithName("ToggleCategory")
            .WithSummary("Toggle category active status");
    }
}
