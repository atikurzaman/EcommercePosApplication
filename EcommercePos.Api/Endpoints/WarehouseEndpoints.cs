using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class WarehouseEndpoints
{
    public static void MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/warehouses").WithTags("Warehouses");

        group.MapGet("/", async (
            [AsParameters] GetWarehouses.Request request,
            [FromServices] GetWarehouses.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetWarehouses")
        .WithSummary("Get paginated warehouses");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetWarehouseById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetWarehouseById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetWarehouseById")
        .WithSummary("Get warehouse by id with counters");

        group.MapPost("/", async (
            [FromBody] CreateWarehouse.Request request,
            [FromServices] CreateWarehouse.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/warehouses/{result.Value?.Id}");
        })
        .WithName("CreateWarehouse")
        .WithSummary("Create a new warehouse");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateWarehouse.Request request,
            [FromServices] UpdateWarehouse.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateWarehouse.Command(
                id, request.Code, request.Name, request.SiteType,
                request.ParentId, request.ContactPerson, request.ManagerName,
                request.AddressLine1, request.AddressLine2, request.City, request.Area,
                request.State, request.PostalCode, request.Country,
                request.Phone, request.Email,
                request.Latitude, request.Longitude,
                request.OpeningTime, request.ClosingTime,
                request.TaxNumber, request.IsDefault, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateWarehouse")
        .WithSummary("Update an existing warehouse");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteWarehouse.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteWarehouse.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteWarehouse")
        .WithSummary("Soft delete a warehouse");

        group.MapPost("/{id:guid}/toggle-active", async (
            Guid id,
            [FromServices] ToggleWarehouseActive.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new ToggleWarehouseActive.Command(id), ct);
            return result.ToHttpResult();
        })
        .WithName("ToggleWarehouseActive")
        .WithSummary("Toggle warehouse active status");

        group.MapGet("/stats", async (
            [FromServices] GetWarehouseStats.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetWarehouseStats.Query(), ct);
            return result.ToHttpResult();
        })
        .WithName("GetWarehouseStats")
        .WithSummary("Get warehouse statistics");
    }
}
