using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class OrderStatusEndpoints
{
    public static void MapOrderStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/order-statuses").WithTags("OrderStatuses");

        group.MapGet("/", async (
            [AsParameters] GetOrderStatuses.Request request,
            [FromServices] GetOrderStatuses.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetOrderStatuses")
        .WithSummary("Get paginated order statuses");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetOrderStatusByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetOrderStatusByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetOrderStatusByCode")
        .WithSummary("Get order status by code");

        group.MapPost("/", async (
            [FromBody] CreateOrderStatus.Request request,
            [FromServices] CreateOrderStatus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/order-statuses/{request.StatusCode}");
        })
        .WithName("CreateOrderStatus")
        .WithSummary("Create a new order status");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateOrderStatus.Request request,
            [FromServices] UpdateOrderStatus.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateOrderStatus.Command(code, request.StatusCode, request.DisplayName, request.Description, request.SortOrder, request.IsTerminal);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateOrderStatus")
        .WithSummary("Update an existing order status");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteOrderStatus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteOrderStatus.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteOrderStatus")
        .WithSummary("Delete an order status");
    }
}
