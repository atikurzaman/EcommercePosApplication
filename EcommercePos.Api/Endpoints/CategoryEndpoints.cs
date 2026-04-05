using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", async (
            [AsParameters] GetCategoriesRequest request, 
            ApplicationDbContext context, 
            CancellationToken ct) =>
        {
            var query = context.Categories
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.Name.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.DisplayOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CategoryResponse(
                    c.Id, c.Name, c.Description, c.ParentCategoryId, 
                    c.IsActive, c.DisplayOrder, c.ImageUrl))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetCategories")
        .WithSummary("Get paginated categories");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = await context.Categories
                .Where(c => c.Id == id && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (category == null)
                return Results.NotFound(new { error = "Category not found" });

            return Results.Ok(new { data = new CategoryResponse(
                category.Id, category.Name, category.Description, 
                category.ParentCategoryId, category.IsActive, 
                category.DisplayOrder, category.ImageUrl) });
        })
        .WithName("GetCategoryById")
        .WithSummary("Get category by id");

        group.MapPost("/", async (CreateCategoryRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = new Categories
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                ParentCategoryId = request.ParentCategoryId,
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive,
                IsFeatured = request.IsFeatured,
                Slug = request.Name.ToLower().Replace(" ", "-"),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/categories/{category.Id}", new { data = category });
        })
        .WithName("CreateCategory")
        .WithSummary("Create a new category");

        group.MapPut("/{id:guid}", async (Guid id, UpdateCategoryRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = await context.Categories.FindAsync(new object[] { id }, ct);
            if (category == null || category.IsDeleted)
                return Results.NotFound(new { error = "Category not found" });

            category.Name = request.Name;
            category.Description = request.Description;
            category.ImageUrl = request.ImageUrl;
            category.ParentCategoryId = request.ParentCategoryId;
            category.DisplayOrder = request.DisplayOrder;
            category.IsActive = request.IsActive;
            category.IsFeatured = request.IsFeatured;
            category.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = category });
        })
        .WithName("UpdateCategory")
        .WithSummary("Update an existing category");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var category = await context.Categories.FindAsync(new object[] { id }, ct);
            if (category == null || category.IsDeleted)
                return Results.NotFound(new { error = "Category not found" });

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteCategory")
        .WithSummary("Soft delete a category");
    }
}

public record GetCategoriesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record CategoryResponse(Guid Id, string Name, string? Description, Guid? ParentCategoryId, bool IsActive, int DisplayOrder, string? ImageUrl);
public record CreateCategoryRequest(string Name, string? Description, string? ImageUrl, Guid? ParentCategoryId, int DisplayOrder, bool IsActive, bool IsFeatured);
public record UpdateCategoryRequest(string Name, string? Description, string? ImageUrl, Guid? ParentCategoryId, int DisplayOrder, bool IsActive, bool IsFeatured);
