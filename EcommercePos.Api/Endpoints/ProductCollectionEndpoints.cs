using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ProductCollectionEndpoints
{
    public static void MapProductCollectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product-collections").WithTags("ProductCollections");

        group.MapGet("/", async (
            [AsParameters] GetProductCollectionsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.ProductCollections
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.Name.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ProductCollectionResponse(
                    c.Id, c.Name, c.Slug, c.Description, c.DisplayOrder, c.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetProductCollections")
        .WithSummary("Get paginated product collections");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var collection = await context.ProductCollections
                .Where(c => c.Id == id && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (collection == null)
                return Results.NotFound(new { error = "Product collection not found" });

            return Results.Ok(new { data = new ProductCollectionResponse(
                collection.Id, collection.Name, collection.Slug, collection.Description, 
                collection.DisplayOrder, collection.IsActive) });
        })
        .WithName("GetProductCollectionById")
        .WithSummary("Get product collection by id");

        group.MapPost("/", async (CreateProductCollectionRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var collection = new ProductCollections
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-"),
                Description = request.Description,
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.ProductCollections.Add(collection);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/product-collections/{collection.Id}", new { data = new ProductCollectionResponse(
                collection.Id, collection.Name, collection.Slug, collection.Description, 
                collection.DisplayOrder, collection.IsActive) });
        })
        .WithName("CreateProductCollection")
        .WithSummary("Create a new product collection");

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductCollectionRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var collection = await context.ProductCollections.FindAsync(new object[] { id }, ct);
            if (collection == null || collection.IsDeleted)
                return Results.NotFound(new { error = "Product collection not found" });

            collection.Name = request.Name;
            collection.Slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-");
            collection.Description = request.Description;
            collection.DisplayOrder = request.DisplayOrder;
            collection.IsActive = request.IsActive;
            collection.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new ProductCollectionResponse(
                collection.Id, collection.Name, collection.Slug, collection.Description, 
                collection.DisplayOrder, collection.IsActive) });
        })
        .WithName("UpdateProductCollection")
        .WithSummary("Update an existing product collection");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var collection = await context.ProductCollections.FindAsync(new object[] { id }, ct);
            if (collection == null || collection.IsDeleted)
                return Results.NotFound(new { error = "Product collection not found" });

            collection.IsDeleted = true;
            collection.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteProductCollection")
        .WithSummary("Soft delete a product collection");
    }
}

public record GetProductCollectionsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record ProductCollectionResponse(
    Guid Id, string Name, string Slug, string? Description, int DisplayOrder, bool IsActive);
public record CreateProductCollectionRequest(
    string Name, string? Description, int DisplayOrder, bool IsActive, string? Slug = null);
public record UpdateProductCollectionRequest(
    string Name, string? Description, int DisplayOrder, bool IsActive, string? Slug = null);
