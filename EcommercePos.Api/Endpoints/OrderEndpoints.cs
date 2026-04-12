using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Order;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", async (
            [AsParameters] GetOrders.Query request,
            GetOrders.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetOrders")
        .WithSummary("Get paginated orders");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetOrderById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetOrderById.Query(id), ct)).ToHttpResult())
        .WithName("GetOrderById")
        .WithSummary("Get order with details");

        group.MapPut("/{id:guid}/status", async (
            Guid id,
            [FromBody] UpdateOrderStatusById.Command command,
            UpdateOrderStatusById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateOrderStatusById.Command(id, command.StatusCode), ct)).ToHttpResult())
        .WithName("UpdateOrderStatusById")
        .WithSummary("Update order status by ID");

        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            [FromBody] CancelOrder.Command command,
            CancelOrder.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new CancelOrder.Command(id, command.Reason), ct)).ToHttpResult())
        .WithName("CancelOrder")
        .WithSummary("Cancel order and restore stock");

        group.MapGet("/stats", async (
            GetOrderStats.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(ct)).ToHttpResult())
        .WithName("GetOrderStats")
        .WithSummary("Get order statistics");
    }
}