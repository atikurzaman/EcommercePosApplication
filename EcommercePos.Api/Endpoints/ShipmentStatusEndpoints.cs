using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class ShipmentStatusEndpoints
{
    public static void MapShipmentStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shipment-statuses").WithTags("ShipmentStatuses");

        group.MapGet("/", async (
            [AsParameters] GetShipmentStatuses.Request request,
            [FromServices] GetShipmentStatuses.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetShipmentStatuses")
        .WithSummary("Get paginated shipment statuses");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetShipmentStatusByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetShipmentStatusByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetShipmentStatusByCode")
        .WithSummary("Get shipment status by code");

        group.MapPost("/", async (
            [FromBody] CreateShipmentStatus.Request request,
            [FromServices] CreateShipmentStatus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/shipment-statuses/{request.StatusCode}");
        })
        .WithName("CreateShipmentStatus")
        .WithSummary("Create a new shipment status");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateShipmentStatus.Request request,
            [FromServices] UpdateShipmentStatus.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateShipmentStatus.Command(code, request.StatusCode, request.DisplayName, request.SortOrder);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateShipmentStatus")
        .WithSummary("Update an existing shipment status");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteShipmentStatus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteShipmentStatus.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteShipmentStatus")
        .WithSummary("Delete a shipment status");
    }
}
