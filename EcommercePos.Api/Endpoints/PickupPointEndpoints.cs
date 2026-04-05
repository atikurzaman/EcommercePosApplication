using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class PickupPointEndpoints
{
    public static void MapPickupPointEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pickup-points").WithTags("PickupPoints");

        group.MapGet("/", async (
            [AsParameters] GetPickupPointsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.PickupPoints
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(p => p.Name.Contains(request.Search) || p.City.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(p => p.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PickupPointResponse(
                    p.Id, p.WarehouseId, p.Name, p.AddressLine1, p.City,
                    p.PostalCode, p.Phone, p.Latitude, p.Longitude,
                    p.OpeningTime, p.ClosingTime, p.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetPickupPoints")
        .WithSummary("Get paginated pickup points");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var point = await context.PickupPoints
                .Where(p => p.Id == id && !p.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (point == null)
                return Results.NotFound(new { error = "Pickup point not found" });

            return Results.Ok(new { data = new PickupPointResponse(
                point.Id, point.WarehouseId, point.Name, point.AddressLine1, point.City,
                point.PostalCode, point.Phone, point.Latitude, point.Longitude,
                point.OpeningTime, point.ClosingTime, point.IsActive) });
        })
        .WithName("GetPickupPointById")
        .WithSummary("Get pickup point by id");

        group.MapPost("/", async (CreatePickupPointRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var point = new PickupPoints
            {
                Id = Guid.NewGuid(),
                WarehouseId = request.WarehouseId,
                Name = request.Name,
                AddressLine1 = request.AddressLine1,
                City = request.City,
                PostalCode = request.PostalCode,
                Phone = request.Phone,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                OpeningTime = request.OpeningTime,
                ClosingTime = request.ClosingTime,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.PickupPoints.Add(point);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/pickup-points/{point.Id}", new { data = new PickupPointResponse(
                point.Id, point.WarehouseId, point.Name, point.AddressLine1, point.City,
                point.PostalCode, point.Phone, point.Latitude, point.Longitude,
                point.OpeningTime, point.ClosingTime, point.IsActive) });
        })
        .WithName("CreatePickupPoint")
        .WithSummary("Create a new pickup point");

        group.MapPut("/{id:guid}", async (Guid id, UpdatePickupPointRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var point = await context.PickupPoints.FindAsync(new object[] { id }, ct);
            if (point == null || point.IsDeleted)
                return Results.NotFound(new { error = "Pickup point not found" });

            point.WarehouseId = request.WarehouseId;
            point.Name = request.Name;
            point.AddressLine1 = request.AddressLine1;
            point.City = request.City;
            point.PostalCode = request.PostalCode;
            point.Phone = request.Phone;
            point.Latitude = request.Latitude;
            point.Longitude = request.Longitude;
            point.OpeningTime = request.OpeningTime;
            point.ClosingTime = request.ClosingTime;
            point.IsActive = request.IsActive;
            point.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new PickupPointResponse(
                point.Id, point.WarehouseId, point.Name, point.AddressLine1, point.City,
                point.PostalCode, point.Phone, point.Latitude, point.Longitude,
                point.OpeningTime, point.ClosingTime, point.IsActive) });
        })
        .WithName("UpdatePickupPoint")
        .WithSummary("Update an existing pickup point");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var point = await context.PickupPoints.FindAsync(new object[] { id }, ct);
            if (point == null || point.IsDeleted)
                return Results.NotFound(new { error = "Pickup point not found" });

            point.IsDeleted = true;
            point.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeletePickupPoint")
        .WithSummary("Soft delete a pickup point");
    }
}

public record GetPickupPointsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record PickupPointResponse(
    Guid Id, Guid? WarehouseId, string Name, string AddressLine1, string City,
    string? PostalCode, string Phone, decimal? Latitude, decimal? Longitude,
    TimeOnly? OpeningTime, TimeOnly? ClosingTime, bool IsActive);
public record CreatePickupPointRequest(
    Guid? WarehouseId, string Name, string AddressLine1, string City,
    string? PostalCode, string Phone, decimal? Latitude, decimal? Longitude,
    TimeOnly? OpeningTime, TimeOnly? ClosingTime, bool IsActive);
public record UpdatePickupPointRequest(
    Guid? WarehouseId, string Name, string AddressLine1, string City,
    string? PostalCode, string Phone, decimal? Latitude, decimal? Longitude,
    TimeOnly? OpeningTime, TimeOnly? ClosingTime, bool IsActive);