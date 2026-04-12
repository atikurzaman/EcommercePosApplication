using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.InventoryAdjustment;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class InventoryAdjustmentEndpoints
{
    public static void MapInventoryAdjustmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory-adjustments").WithTags("InventoryAdjustments");

        group.MapGet("/", async (
            [AsParameters] GetInventoryAdjustments.Query request,
            GetInventoryAdjustments.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetInventoryAdjustments")
        .WithSummary("Get paginated inventory adjustments");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetInventoryAdjustmentById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetInventoryAdjustmentById.Query(id), ct)).ToHttpResult())
        .WithName("GetInventoryAdjustmentById")
        .WithSummary("Get inventory adjustment with lines");

        group.MapPost("/", async (
            [FromBody] CreateInventoryAdjustment.Command command,
            CreateInventoryAdjustment.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/inventory-adjustments"))
        .AddEndpointFilter<ValidationFilter<CreateInventoryAdjustment.Command>>()
        .WithName("CreateInventoryAdjustment")
        .WithSummary("Create inventory adjustment");

        group.MapPost("/{id:guid}/approve", async (
            Guid id,
            [FromQuery] Guid? approvedByUserId,
            ApproveInventoryAdjustment.Handler handler,
            CancellationToken ct) =>
        {
            if (!approvedByUserId.HasValue || approvedByUserId.Value == Guid.Empty)
                return Result.Failure(Error.Validation("approvedByUserId is required")).ToHttpResult();

            var result = await handler.Handle(new ApproveInventoryAdjustment.Command(id, approvedByUserId.Value), ct);
            return result.ToHttpResult();
        })
        .WithName("ApproveInventoryAdjustment")
        .WithSummary("Approve inventory adjustment");
    }
}