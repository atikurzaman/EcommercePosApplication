using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class StockMovementTypeEndpoints
{
    public static void MapStockMovementTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-movement-types").WithTags("StockMovementTypes");

        group.MapGet("/", async (
            [AsParameters] GetStockMovementTypes.Request request,
            [FromServices] GetStockMovementTypes.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetStockMovementTypes")
        .WithSummary("Get paginated stock movement types");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetStockMovementTypeByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetStockMovementTypeByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetStockMovementTypeByCode")
        .WithSummary("Get stock movement type by code");

        group.MapPost("/", async (
            [FromBody] CreateStockMovementType.Request request,
            [FromServices] CreateStockMovementType.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/stock-movement-types/{request.TypeCode}");
        })
        .AddEndpointFilter<ValidationFilter<CreateStockMovementType.Request>>()
        .WithName("CreateStockMovementType")
        .WithSummary("Create a new stock movement type");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateStockMovementType.Request request,
            [FromServices] UpdateStockMovementType.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateStockMovementType.Command(code, request.TypeCode, request.DisplayName, request.IsInbound);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .AddEndpointFilter<ValidationFilter<UpdateStockMovementType.Request>>()
        .WithName("UpdateStockMovementType")
        .WithSummary("Update an existing stock movement type");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteStockMovementType.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteStockMovementType.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteStockMovementType")
        .WithSummary("Delete a stock movement type");
    }
}
