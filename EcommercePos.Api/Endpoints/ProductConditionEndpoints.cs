using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ProductConditionEndpoints
{
    public static void MapProductConditionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product-conditions").WithTags("ProductConditions");

        group.MapGet("/", async (
            [AsParameters] GetProductConditionsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.ProductConditions.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.ConditionCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.ConditionCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ProductConditionResponse(c.ConditionCode, c.DisplayName))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetProductConditions")
        .WithSummary("Get paginated product conditions");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var condition = await context.ProductConditions
                .Where(c => c.ConditionCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (condition == null)
                return Results.NotFound(new { error = "Product condition not found" });

            return Results.Ok(new { data = new ProductConditionResponse(condition.ConditionCode, condition.DisplayName) });
        })
        .WithName("GetProductConditionByCode")
        .WithSummary("Get product condition by code");

        group.MapPost("/", async (CreateProductConditionRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.ProductConditions.AnyAsync(c => c.ConditionCode == request.ConditionCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Product condition code already exists" });

            var condition = new ProductConditions
            {
                ConditionCode = request.ConditionCode,
                DisplayName = request.DisplayName
            };

            context.ProductConditions.Add(condition);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/product-conditions/{condition.ConditionCode}", new { data = new ProductConditionResponse(
                condition.ConditionCode, condition.DisplayName) });
        })
        .WithName("CreateProductCondition")
        .WithSummary("Create a new product condition");

        group.MapPut("/{code}", async (string code, UpdateProductConditionRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var condition = await context.ProductConditions.FirstOrDefaultAsync(c => c.ConditionCode == code, ct);
            if (condition == null)
                return Results.NotFound(new { error = "Product condition not found" });

            if (condition.ConditionCode != request.ConditionCode)
            {
                var exists = await context.ProductConditions.AnyAsync(c => c.ConditionCode == request.ConditionCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Product condition code already exists" });
            }

            condition.ConditionCode = request.ConditionCode;
            condition.DisplayName = request.DisplayName;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new ProductConditionResponse(
                condition.ConditionCode, condition.DisplayName) });
        })
        .WithName("UpdateProductCondition")
        .WithSummary("Update an existing product condition");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var condition = await context.ProductConditions.FirstOrDefaultAsync(c => c.ConditionCode == code, ct);
            if (condition == null)
                return Results.NotFound(new { error = "Product condition not found" });

            context.ProductConditions.Remove(condition);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteProductCondition")
        .WithSummary("Delete a product condition");
    }
}

public record GetProductConditionsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record ProductConditionResponse(string ConditionCode, string DisplayName);
public record CreateProductConditionRequest(string ConditionCode, string DisplayName);
public record UpdateProductConditionRequest(string ConditionCode, string DisplayName);
