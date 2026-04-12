using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Category.Queries;
using EcommercePos.Application.Features.Category.Commands;
using EcommercePos.Api.Extensions;
using EcommercePos.Shared.Common;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

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

        group.MapGet("/tree", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var categories = await context.Categories
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync(ct);

            var tree = BuildCategoryTree(categories.Select(c => new CategoryTreeItem(
                c.Id, c.Name, c.Slug, c.ParentCategoryId, c.DisplayOrder, c.IsActive, c.ImageUrl, null)).ToList());
            return Results.Ok(new { data = tree });
        })
        .WithName("GetCategoryTree")
        .WithSummary("Get category tree for hierarchical display");

        group.MapGet("/flat", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var categories = await context.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.ParentCategoryId, c.DisplayOrder })
                .ToListAsync(ct);

            return Results.Ok(new { data = categories });
        })
        .WithName("GetCategoryFlat")
        .WithSummary("Get flattened categories for dropdowns");

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
                request.Name, request.Slug ?? request.Name.ToLower().Replace(" ", "-"),
                request.Description, request.ImageUrl,
                request.ParentCategoryId, request.DisplayOrder, request.IsFeatured,
                request.IsActive, request.MetaTitle, request.MetaDescription);
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
                id, request.Name, request.Slug, request.Description, request.ImageUrl,
                request.ParentCategoryId, request.DisplayOrder, request.IsFeatured,
                request.IsActive, request.MetaTitle, request.MetaDescription);
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

        group.MapPatch("/{id:guid}/toggle", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = await context.Categories.FindAsync(new object[] { id }, ct);
            if (category == null || category.IsDeleted)
                return Results.NotFound(new { error = "Category not found" });

            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            return Results.Ok(new { data = new { category.Id, category.IsActive } });
        })
        .WithName("ToggleCategory")
        .WithSummary("Toggle category active status");
    }

    private static List<CategoryTreeItem> BuildCategoryTree(List<CategoryTreeItem> flat)
    {
        var dict = flat.ToDictionary(c => c.Id);
        var result = new List<CategoryTreeItem>();
        
        foreach (var item in flat)
        {
            if (item.ParentCategoryId == null || !dict.ContainsKey(item.ParentCategoryId.Value))
            {
                result.Add(item);
            }
        }
        
        void AddChildren(CategoryTreeItem parent)
        {
            var children = flat.Where(c => c.ParentCategoryId == parent.Id).ToList();
            foreach (var child in children)
            {
                AddChildren(child);
            }
            if (children.Any())
            {
                result.Add(new CategoryTreeItem(
                    parent.Id, parent.Name, parent.Slug, parent.ParentCategoryId,
                    parent.DisplayOrder, parent.IsActive, parent.ImageUrl, children));
            }
        }
        
        return result;
    }
}

public record CategoryTreeItem(
    Guid Id, string Name, string? Slug, Guid? ParentCategoryId, 
    int DisplayOrder, bool IsActive, string? ImageUrl,
    List<CategoryTreeItem>? Children);