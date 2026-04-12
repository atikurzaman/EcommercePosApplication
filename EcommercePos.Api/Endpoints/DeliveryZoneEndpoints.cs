using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.DeliveryZone;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class DeliveryZoneEndpoints
{
    public static void MapDeliveryZoneEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/delivery-zones").WithTags("DeliveryZones");

        group.MapGet("/", async (
            [AsParameters] GetDeliveryZones.Query request,
            GetDeliveryZones.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetDeliveryZones")
        .WithSummary("Get paginated delivery zones");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetDeliveryZoneById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetDeliveryZoneById.Query(id), ct)).ToHttpResult())
        .WithName("GetDeliveryZoneById")
        .WithSummary("Get delivery zone by id");

        group.MapPost("/", async (
            [FromBody] CreateDeliveryZone.Command command,
            CreateDeliveryZone.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/delivery-zones"))
        .AddEndpointFilter<ValidationFilter<CreateDeliveryZone.Command>>()
        .WithName("CreateDeliveryZone")
        .WithSummary("Create a new delivery zone");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateDeliveryZone.Command command,
            UpdateDeliveryZone.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateDeliveryZone.Command(
                id,
                command.Name,
                command.Description,
                command.IsActive,
                command.BaseDeliveryCost,
                command.FreeDeliveryThreshold,
                command.MinDeliveryDays,
                command.MaxDeliveryDays), ct)).ToHttpResult())
        .AddEndpointFilter<ValidationFilter<UpdateDeliveryZone.Command>>()
        .WithName("UpdateDeliveryZone")
        .WithSummary("Update an existing delivery zone");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteDeliveryZone.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteDeliveryZone.Command(id), ct)).ToNoContentResult())
        .WithName("DeleteDeliveryZone")
        .WithSummary("Soft delete a delivery zone");
    }
}
