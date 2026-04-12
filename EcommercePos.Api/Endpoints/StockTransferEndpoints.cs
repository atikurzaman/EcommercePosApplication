using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.StockTransfer;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class StockTransferEndpoints
{
    public static void MapStockTransferEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-transfers").WithTags("StockTransfers");

        group.MapGet("/", async (
            [AsParameters] GetStockTransfers.Query request,
            GetStockTransfers.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetStockTransfers")
        .WithSummary("Get paginated stock transfers");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetStockTransferById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetStockTransferById.Query(id), ct)).ToHttpResult())
        .WithName("GetStockTransferById")
        .WithSummary("Get stock transfer with lines");

        group.MapPost("/", async (
            [FromBody] CreateStockTransfer.Command command,
            CreateStockTransfer.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/stock-transfers"))
        .AddEndpointFilter<ValidationFilter<CreateStockTransfer.Command>>()
        .WithName("CreateStockTransfer")
        .WithSummary("Create stock transfer");

        group.MapPost("/{id:guid}/receive", async (
            Guid id,
            ReceiveStockTransfer.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ReceiveStockTransfer.Command(id), ct)).ToHttpResult())
        .WithName("ReceiveStockTransfer")
        .WithSummary("Receive stock transfer");
    }
}