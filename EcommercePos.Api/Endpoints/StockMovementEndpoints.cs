using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Inventory;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class StockMovementEndpoints
{
    public static void MapStockMovementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-movements").WithTags("StockMovements");

        group.MapGet("/", async (
            [AsParameters] GetAllStockMovements.Query request,
            GetAllStockMovements.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetStockMovements")
        .WithSummary("Get paginated stock movements");

        group.MapGet("/types", async (
            GetMovementTypes.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(ct)).ToHttpResult())
        .WithName("GetMovementTypes")
        .WithSummary("Get stock movement types");
    }
}