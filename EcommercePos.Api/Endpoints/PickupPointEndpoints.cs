using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.PickupPoint;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class PickupPointEndpoints
{
    public static void MapPickupPointEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pickup-points").WithTags("PickupPoints");

        group.MapGet("/", async (
            [AsParameters] GetPickupPoints.Query request,
            GetPickupPoints.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetPickupPoints")
        .WithSummary("Get paginated pickup points");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetPickupPointById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetPickupPointById.Query(id), ct)).ToHttpResult())
        .WithName("GetPickupPointById")
        .WithSummary("Get pickup point by id");

        group.MapPost("/", async (
            [FromBody] CreatePickupPoint.Command command,
            CreatePickupPoint.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/pickup-points"))
        .AddEndpointFilter<ValidationFilter<CreatePickupPoint.Command>>()
        .WithName("CreatePickupPoint")
        .WithSummary("Create a new pickup point");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdatePickupPoint.Command command,
            UpdatePickupPoint.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdatePickupPoint.Command(
                id,
                command.WarehouseId,
                command.Name,
                command.AddressLine1,
                command.City,
                command.PostalCode,
                command.Phone,
                command.Latitude,
                command.Longitude,
                command.OpeningTime,
                command.ClosingTime,
                command.IsActive), ct)).ToHttpResult())
        .AddEndpointFilter<ValidationFilter<UpdatePickupPoint.Command>>()
        .WithName("UpdatePickupPoint")
        .WithSummary("Update an existing pickup point");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeletePickupPoint.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeletePickupPoint.Command(id), ct)).ToNoContentResult())
        .WithName("DeletePickupPoint")
        .WithSummary("Soft delete a pickup point");
    }
}