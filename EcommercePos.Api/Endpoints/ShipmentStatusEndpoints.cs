using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ShipmentStatusEndpoints
{
    public static void MapShipmentStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shipment-statuses").WithTags("ShipmentStatuses");

        group.MapGet("/", async (
            [AsParameters] GetShipmentStatusesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.ShipmentStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.StatusCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ShipmentStatusResponse(c.StatusCode, c.DisplayName, c.SortOrder))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetShipmentStatuses")
        .WithSummary("Get paginated shipment statuses");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.ShipmentStatuses
                .Where(c => c.StatusCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (status == null)
                return Results.NotFound(new { error = "Shipment status not found" });

            return Results.Ok(new { data = new ShipmentStatusResponse(status.StatusCode, status.DisplayName, status.SortOrder) });
        })
        .WithName("GetShipmentStatusByCode")
        .WithSummary("Get shipment status by code");

        group.MapPost("/", async (CreateShipmentStatusRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.ShipmentStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Shipment status code already exists" });

            var status = new ShipmentStatuses
            {
                StatusCode = request.StatusCode,
                DisplayName = request.DisplayName,
                SortOrder = request.SortOrder
            };

            context.ShipmentStatuses.Add(status);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/shipment-statuses/{status.StatusCode}", new { data = new ShipmentStatusResponse(
                status.StatusCode, status.DisplayName, status.SortOrder) });
        })
        .WithName("CreateShipmentStatus")
        .WithSummary("Create a new shipment status");

        group.MapPut("/{code}", async (string code, UpdateShipmentStatusRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.ShipmentStatuses.FirstOrDefaultAsync(c => c.StatusCode == code, ct);
            if (status == null)
                return Results.NotFound(new { error = "Shipment status not found" });

            if (status.StatusCode != request.StatusCode)
            {
                var exists = await context.ShipmentStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Shipment status code already exists" });
            }

            status.StatusCode = request.StatusCode;
            status.DisplayName = request.DisplayName;
            status.SortOrder = request.SortOrder;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new ShipmentStatusResponse(
                status.StatusCode, status.DisplayName, status.SortOrder) });
        })
        .WithName("UpdateShipmentStatus")
        .WithSummary("Update an existing shipment status");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.ShipmentStatuses.FirstOrDefaultAsync(c => c.StatusCode == code, ct);
            if (status == null)
                return Results.NotFound(new { error = "Shipment status not found" });

            context.ShipmentStatuses.Remove(status);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteShipmentStatus")
        .WithSummary("Delete a shipment status");
    }
}

public record GetShipmentStatusesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record ShipmentStatusResponse(string StatusCode, string DisplayName, byte SortOrder);
public record CreateShipmentStatusRequest(string StatusCode, string DisplayName, byte SortOrder);
public record UpdateShipmentStatusRequest(string StatusCode, string DisplayName, byte SortOrder);
