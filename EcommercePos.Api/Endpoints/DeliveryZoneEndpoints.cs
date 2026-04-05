using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class DeliveryZoneEndpoints
{
    public static void MapDeliveryZoneEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/delivery-zones").WithTags("DeliveryZones");

        group.MapGet("/", async (
            [AsParameters] GetDeliveryZonesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.DeliveryZones
                .Where(z => !z.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(z => z.Name.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(z => z.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(z => new DeliveryZoneResponse(
                    z.Id, z.Name, z.Description, z.IsActive,
                    z.BaseDeliveryCost, z.FreeDeliveryThreshold,
                    z.MinDeliveryDays, z.MaxDeliveryDays))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetDeliveryZones")
        .WithSummary("Get paginated delivery zones");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var zone = await context.DeliveryZones
                .Where(z => z.Id == id && !z.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (zone == null)
                return Results.NotFound(new { error = "Delivery zone not found" });

            return Results.Ok(new { data = new DeliveryZoneResponse(
                zone.Id, zone.Name, zone.Description, zone.IsActive,
                zone.BaseDeliveryCost, zone.FreeDeliveryThreshold,
                zone.MinDeliveryDays, zone.MaxDeliveryDays) });
        })
        .WithName("GetDeliveryZoneById")
        .WithSummary("Get delivery zone by id");

        group.MapPost("/", async (CreateDeliveryZoneRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var zone = new DeliveryZones
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                BaseDeliveryCost = request.BaseDeliveryCost,
                FreeDeliveryThreshold = request.FreeDeliveryThreshold,
                MinDeliveryDays = request.MinDeliveryDays,
                MaxDeliveryDays = request.MaxDeliveryDays,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.DeliveryZones.Add(zone);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/delivery-zones/{zone.Id}", new { data = new DeliveryZoneResponse(
                zone.Id, zone.Name, zone.Description, zone.IsActive,
                zone.BaseDeliveryCost, zone.FreeDeliveryThreshold,
                zone.MinDeliveryDays, zone.MaxDeliveryDays) });
        })
        .WithName("CreateDeliveryZone")
        .WithSummary("Create a new delivery zone");

        group.MapPut("/{id:guid}", async (Guid id, UpdateDeliveryZoneRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var zone = await context.DeliveryZones.FindAsync(new object[] { id }, ct);
            if (zone == null || zone.IsDeleted)
                return Results.NotFound(new { error = "Delivery zone not found" });

            zone.Name = request.Name;
            zone.Description = request.Description;
            zone.IsActive = request.IsActive;
            zone.BaseDeliveryCost = request.BaseDeliveryCost;
            zone.FreeDeliveryThreshold = request.FreeDeliveryThreshold;
            zone.MinDeliveryDays = request.MinDeliveryDays;
            zone.MaxDeliveryDays = request.MaxDeliveryDays;
            zone.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new DeliveryZoneResponse(
                zone.Id, zone.Name, zone.Description, zone.IsActive,
                zone.BaseDeliveryCost, zone.FreeDeliveryThreshold,
                zone.MinDeliveryDays, zone.MaxDeliveryDays) });
        })
        .WithName("UpdateDeliveryZone")
        .WithSummary("Update an existing delivery zone");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var zone = await context.DeliveryZones.FindAsync(new object[] { id }, ct);
            if (zone == null || zone.IsDeleted)
                return Results.NotFound(new { error = "Delivery zone not found" });

            zone.IsDeleted = true;
            zone.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteDeliveryZone")
        .WithSummary("Soft delete a delivery zone");
    }
}

public record GetDeliveryZonesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record DeliveryZoneResponse(
    Guid Id, string Name, string? Description, bool IsActive,
    decimal BaseDeliveryCost, decimal? FreeDeliveryThreshold,
    int? MinDeliveryDays, int? MaxDeliveryDays);
public record CreateDeliveryZoneRequest(
    string Name, string? Description, bool IsActive,
    decimal BaseDeliveryCost, decimal? FreeDeliveryThreshold,
    int? MinDeliveryDays, int? MaxDeliveryDays);
public record UpdateDeliveryZoneRequest(
    string Name, string? Description, bool IsActive,
    decimal BaseDeliveryCost, decimal? FreeDeliveryThreshold,
    int? MinDeliveryDays, int? MaxDeliveryDays);
