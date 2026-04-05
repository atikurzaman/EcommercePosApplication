using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ShippingMethodEndpoints
{
    public static void MapShippingMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shipping-methods").WithTags("ShippingMethods");

        group.MapGet("/", async (
            [AsParameters] GetShippingMethodsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.ShippingMethods
                .Where(s => !s.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(s => s.Name.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(s => s.DisplayOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new ShippingMethodResponse(
                    s.Id, s.Name, s.Description, s.CarrierName, s.BaseCost,
                    s.CostPerKg, s.EstimatedDaysMin, s.EstimatedDaysMax, s.IsActive, s.IsFreeShipping))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetShippingMethods")
        .WithSummary("Get paginated shipping methods");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var method = await context.ShippingMethods
                .Where(s => s.Id == id && !s.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (method == null)
                return Results.NotFound(new { error = "Shipping method not found" });

            return Results.Ok(new { data = new ShippingMethodResponse(
                method.Id, method.Name, method.Description, method.CarrierName, method.BaseCost,
                method.CostPerKg, method.EstimatedDaysMin, method.EstimatedDaysMax, method.IsActive, method.IsFreeShipping) });
        })
        .WithName("GetShippingMethodById")
        .WithSummary("Get shipping method by id");

        group.MapPost("/", async (CreateShippingMethodRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var method = new ShippingMethods
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CarrierName = request.CarrierName,
                BaseCost = request.BaseCost,
                CostPerKg = request.CostPerKg,
                EstimatedDaysMin = request.EstimatedDaysMin,
                EstimatedDaysMax = request.EstimatedDaysMax,
                IsActive = request.IsActive,
                IsFreeShipping = request.IsFreeShipping,
                FreeShippingThreshold = request.FreeShippingThreshold,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.ShippingMethods.Add(method);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/shipping-methods/{method.Id}", new { data = new ShippingMethodResponse(
                method.Id, method.Name, method.Description, method.CarrierName, method.BaseCost,
                method.CostPerKg, method.EstimatedDaysMin, method.EstimatedDaysMax, method.IsActive, method.IsFreeShipping) });
        })
        .WithName("CreateShippingMethod")
        .WithSummary("Create a new shipping method");

        group.MapPut("/{id:guid}", async (Guid id, UpdateShippingMethodRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var method = await context.ShippingMethods.FindAsync(new object[] { id }, ct);
            if (method == null || method.IsDeleted)
                return Results.NotFound(new { error = "Shipping method not found" });

            method.Name = request.Name;
            method.Description = request.Description;
            method.CarrierName = request.CarrierName;
            method.BaseCost = request.BaseCost;
            method.CostPerKg = request.CostPerKg;
            method.EstimatedDaysMin = request.EstimatedDaysMin;
            method.EstimatedDaysMax = request.EstimatedDaysMax;
            method.IsActive = request.IsActive;
            method.IsFreeShipping = request.IsFreeShipping;
            method.FreeShippingThreshold = request.FreeShippingThreshold;
            method.DisplayOrder = request.DisplayOrder;
            method.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new ShippingMethodResponse(
                method.Id, method.Name, method.Description, method.CarrierName, method.BaseCost,
                method.CostPerKg, method.EstimatedDaysMin, method.EstimatedDaysMax, method.IsActive, method.IsFreeShipping) });
        })
        .WithName("UpdateShippingMethod")
        .WithSummary("Update an existing shipping method");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var method = await context.ShippingMethods.FindAsync(new object[] { id }, ct);
            if (method == null || method.IsDeleted)
                return Results.NotFound(new { error = "Shipping method not found" });

            method.IsDeleted = true;
            method.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteShippingMethod")
        .WithSummary("Soft delete a shipping method");
    }
}

public record GetShippingMethodsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record ShippingMethodResponse(
    Guid Id, string Name, string? Description, string? CarrierName, decimal BaseCost,
    decimal CostPerKg, int EstimatedDaysMin, int EstimatedDaysMax, bool IsActive, bool IsFreeShipping);
public record CreateShippingMethodRequest(
    string Name, string? Description, string? CarrierName, decimal BaseCost,
    decimal CostPerKg, int EstimatedDaysMin, int EstimatedDaysMax, bool IsActive, bool IsFreeShipping,
    decimal? FreeShippingThreshold, int DisplayOrder);
public record UpdateShippingMethodRequest(
    string Name, string? Description, string? CarrierName, decimal BaseCost,
    decimal CostPerKg, int EstimatedDaysMin, int EstimatedDaysMax, bool IsActive, bool IsFreeShipping,
    decimal? FreeShippingThreshold, int DisplayOrder);
