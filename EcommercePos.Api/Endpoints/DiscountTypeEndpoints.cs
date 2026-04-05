using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class DiscountTypeEndpoints
{
    public static void MapDiscountTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/discount-types").WithTags("DiscountTypes");

        group.MapGet("/", async (
            [AsParameters] GetDiscountTypesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.DiscountTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.TypeCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.TypeCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new DiscountTypeResponse(c.TypeCode, c.DisplayName))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetDiscountTypes")
        .WithSummary("Get paginated discount types");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var discountType = await context.DiscountTypes
                .Where(c => c.TypeCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (discountType == null)
                return Results.NotFound(new { error = "Discount type not found" });

            return Results.Ok(new { data = new DiscountTypeResponse(discountType.TypeCode, discountType.DisplayName) });
        })
        .WithName("GetDiscountTypeByCode")
        .WithSummary("Get discount type by code");

        group.MapPost("/", async (CreateDiscountTypeRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.DiscountTypes.AnyAsync(c => c.TypeCode == request.TypeCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Discount type code already exists" });

            var discountType = new DiscountTypes
            {
                TypeCode = request.TypeCode,
                DisplayName = request.DisplayName
            };

            context.DiscountTypes.Add(discountType);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/discount-types/{discountType.TypeCode}", new { data = new DiscountTypeResponse(
                discountType.TypeCode, discountType.DisplayName) });
        })
        .WithName("CreateDiscountType")
        .WithSummary("Create a new discount type");

        group.MapPut("/{code}", async (string code, UpdateDiscountTypeRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var discountType = await context.DiscountTypes.FirstOrDefaultAsync(c => c.TypeCode == code, ct);
            if (discountType == null)
                return Results.NotFound(new { error = "Discount type not found" });

            if (discountType.TypeCode != request.TypeCode)
            {
                var exists = await context.DiscountTypes.AnyAsync(c => c.TypeCode == request.TypeCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Discount type code already exists" });
            }

            discountType.TypeCode = request.TypeCode;
            discountType.DisplayName = request.DisplayName;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new DiscountTypeResponse(
                discountType.TypeCode, discountType.DisplayName) });
        })
        .WithName("UpdateDiscountType")
        .WithSummary("Update an existing discount type");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var discountType = await context.DiscountTypes.FirstOrDefaultAsync(c => c.TypeCode == code, ct);
            if (discountType == null)
                return Results.NotFound(new { error = "Discount type not found" });

            context.DiscountTypes.Remove(discountType);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteDiscountType")
        .WithSummary("Delete a discount type");
    }
}

public record GetDiscountTypesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record DiscountTypeResponse(string TypeCode, string DisplayName);
public record CreateDiscountTypeRequest(string TypeCode, string DisplayName);
public record UpdateDiscountTypeRequest(string TypeCode, string DisplayName);
