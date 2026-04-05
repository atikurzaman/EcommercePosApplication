using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (
            [AsParameters] PaginationParams request,
            ApplicationDbContext db,
            CancellationToken ct) =>
        {
            var query = db.Products
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(p => p.Name.Contains(request.Search) || p.Sku.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Sku,
                    p.SalePrice,
                    p.IsActive
                })
                .ToListAsync(ct);

            return Results.Ok(new { items, totalCount, request.PageIndex, request.PageSize });
        });

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext db, CancellationToken ct) =>
        {
            var item = await db.Products
                .Where(p => p.Id == id && !p.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            return item is null ? Results.NotFound() : Results.Ok(item);
        });

        group.MapPost("/", async (CreateProductRequest request, ApplicationDbContext db, CancellationToken ct) =>
        {
            var exists = await db.Products.AnyAsync(p => p.Sku == request.Sku && !p.IsDeleted, ct);
            if (exists) return Results.Conflict($"Product with SKU '{request.Sku}' already exists.");

            var product = new Products
            {
                Name = request.Name,
                Sku = request.Sku,
                SalePrice = request.Price,
                IsActive = true
            };

            db.Products.Add(product);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/products/{product.Id}", product);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, ApplicationDbContext db, CancellationToken ct) =>
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);
            if (product is null) return Results.NotFound();

            product.Name = request.Name;
            product.SalePrice = request.Price;

            await db.SaveChangesAsync(ct);
            return Results.Ok(product);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext db, CancellationToken ct) =>
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);
            if (product is null) return Results.NotFound();

            product.IsDeleted = true;
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });
    }
}

public record CreateProductRequest(string Name, string Sku, decimal Price);
public record UpdateProductRequest(string Name, decimal Price);

public class PaginationParams
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
}