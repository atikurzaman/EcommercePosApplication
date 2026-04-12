using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Inventory;
using EcommercePos.Api.Extensions;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class StockItemEndpoints
{
    public static void MapStockItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-items").WithTags("StockItems");

        group.MapGet("/", async (
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromQuery] string? search,
            [FromQuery] Guid? warehouseId,
            [FromQuery] Guid? categoryId,
            GetStockItems.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetStockItems.Query(pageIndex, pageSize, search, warehouseId, null, categoryId, null);
            var result = await handler.Handle(query, ct);
            return result.ToPagedResult();
        })
        .WithName("GetStockItems")
        .WithSummary("Get paginated stock items");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetStockItemById.Handler itemHandler,
            GetStockMovements.Handler movementHandler,
            CancellationToken ct) =>
        {
            var itemResult = await itemHandler.Handle(new GetStockItemById.Query(id), ct);
            if (!itemResult.IsSuccess)
                return itemResult.ToHttpResult();

            var movementResult = await movementHandler.Handle(new GetStockMovements.Query(id), ct);
            if (!movementResult.IsSuccess)
                return movementResult.ToHttpResult();

            return Results.Ok(ApiResponse<object>.Ok(new
            {
                item = itemResult.Value,
                history = movementResult.Value
            }));
        })
        .WithName("GetStockItemById")
        .WithSummary("Get stock item with history");

        group.MapGet("/low-stock", async (
            [FromQuery] Guid? warehouseId,
            GetLowStockItems.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetLowStockItems.Query(warehouseId), ct)).ToHttpResult())
        .WithName("GetLowStockItems")
        .WithSummary("Get items below reorder level");

        group.MapPut("/{id:guid}/reorder-level", async (
            Guid id,
            [FromBody] decimal reorderLevel,
            UpdateReorderLevel.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateReorderLevel.Command(id, reorderLevel), ct)).ToHttpResult())
        .WithName("UpdateReorderLevel")
        .WithSummary("Update reorder level");
    }
}