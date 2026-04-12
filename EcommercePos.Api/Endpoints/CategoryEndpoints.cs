using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Category;
using EcommercePos.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

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

        group.MapGet("/tree", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var categories = await context.Categories
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .Select(c => new CategoryTreeItem(c.Id, c.Name, c.Slug, c.ParentCategoryId,
                    c.DisplayOrder, c.IsActive, c.ImageUrl, null))
                .ToListAsync(ct);

            return Results.Ok(new { data = BuildCategoryTree(categories) });
        })
        .WithName("GetCategoryTree")
        .WithSummary("Get hierarchical category tree");

        group.MapGet("/flat", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var categories = await context.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, c.ParentCategoryId, c.DisplayOrder })
                .ToListAsync(ct);

            return Results.Ok(new { data = categories });
        })
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
            .WithName("CreateCategory")
            .WithSummary("Create a new category");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateCategoryBody body,
            UpdateCategory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateCategory.Command(
                id, body.Name, body.Slug, body.Description, body.ImageUrl,
                body.ParentCategoryId, body.DisplayOrder, body.IsFeatured,
                body.IsActive, body.MetaTitle, body.MetaDescription), ct)).ToHttpResult())
            .WithName("UpdateCategory")
            .WithSummary("Update an existing category");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteCategory.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteCategory.Command(id), ct)).ToNoContentResult())
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
        var roots = flat.Where(c => c.ParentCategoryId == null ||
            !flat.Any(p => p.Id == c.ParentCategoryId)).ToList();

        static List<CategoryTreeItem> GetChildren(Guid parentId, List<CategoryTreeItem> all) =>
            all.Where(c => c.ParentCategoryId == parentId)
               .Select(c => c with { Children = GetChildren(c.Id, all) })
               .ToList();

        return roots.Select(r => r with { Children = GetChildren(r.Id, flat) }).ToList();
    }
}

public record CategoryTreeItem(
    Guid Id, string Name, string? Slug, Guid? ParentCategoryId,
    int DisplayOrder, bool IsActive, string? ImageUrl, List<CategoryTreeItem>? Children);

public record UpdateCategoryBody(
    string Name, string? Slug, string? Description, string? ImageUrl,
    Guid? ParentCategoryId, int DisplayOrder, bool IsFeatured, bool IsActive,
    string? MetaTitle, string? MetaDescription);
