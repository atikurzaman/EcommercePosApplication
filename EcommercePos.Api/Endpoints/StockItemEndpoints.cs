using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class StockItemEndpoints
{
    public static void MapStockItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-items").WithTags("StockItems");

        group.MapGet("/", async (
            [AsParameters] GetStockItemsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.StockItems
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Where(s => !s.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(s => s.Product.Name.Contains(request.Search));
            }

            if (!string.IsNullOrWhiteSpace(request.WarehouseId))
            {
                query = query.Where(s => s.WarehouseId == Guid.Parse(request.WarehouseId));
            }

            if (!string.IsNullOrWhiteSpace(request.CategoryId))
            {
                query = query.Where(s => s.Product.CategoryId == Guid.Parse(request.CategoryId));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(s => s.Product.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new StockItemResponse(
                    s.Id, s.ProductId, s.Product.Name, s.VariantId,
                    s.WarehouseId, s.Warehouse.Name,
                    s.QuantityOnHand, s.ReservedQuantity, s.AverageCostPrice,
                    s.ReorderLevel ?? 0, s.LastUpdatedAt))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetStockItems")
        .WithSummary("Get paginated stock items");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var item = await context.StockItems
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Include(s => s.StockMovements.OrderByDescending(m => m.OccurredAt).Take(20))
                .ThenInclude(m => m.MovementTypeCodeNavigation)
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Results.NotFound(new { error = "Stock item not found" });

            var response = new StockItemDetailResponse(
                item.Id, item.ProductId, item.Product.Name, item.VariantId,
                item.WarehouseId, item.Warehouse.Name,
                item.QuantityOnHand, item.ReservedQuantity, item.AverageCostPrice,
                item.ReorderLevel ?? 0, item.LastUpdatedAt,
                item.StockMovements.Select(m => new StockMovementHistoryItem(
                    m.Id, m.MovementTypeCodeNavigation.TypeCode, m.QuantityIn, m.QuantityOut,
                    m.BalanceAfter, m.ReferenceNumber, m.OccurredAt)).ToList());

            return Results.Ok(new { data = response });
        })
        .WithName("GetStockItemById")
        .WithSummary("Get stock item with history");

        group.MapGet("/low-stock", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var items = await context.StockItems
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Where(s => !s.IsDeleted && s.ReorderLevel.HasValue && s.QuantityOnHand <= s.ReorderLevel)
                .OrderBy(s => s.QuantityOnHand)
                .Select(s => new StockItemResponse(
                    s.Id, s.ProductId, s.Product.Name, s.VariantId,
                    s.WarehouseId, s.Warehouse.Name,
                    s.QuantityOnHand, s.ReservedQuantity, s.AverageCostPrice,
                    s.ReorderLevel ?? 0, s.LastUpdatedAt))
                .ToListAsync(ct);

            return Results.Ok(new { data = items });
        })
        .WithName("GetLowStockItems")
        .WithSummary("Get items below reorder level");

        group.MapPut("/{id:guid}/reorder-level", async (Guid id, UpdateReorderLevelRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var item = await context.StockItems.FindAsync(new object[] { id }, ct);
            if (item == null || item.IsDeleted)
                return Results.NotFound(new { error = "Stock item not found" });

            item.ReorderLevel = request.ReorderLevel;
            item.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            return Results.Ok(new { data = new { item.Id, item.ReorderLevel } });
        })
        .WithName("UpdateReorderLevel")
        .WithSummary("Update reorder level");
    }
}

public record GetStockItemsRequest(
    int PageIndex = 0, int PageSize = 10, string? Search = null,
    string? WarehouseId = null, string? CategoryId = null);

public record StockItemResponse(
    Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
    Guid WarehouseId, string WarehouseName,
    decimal QuantityOnHand, decimal ReservedQuantity, decimal AverageCostPrice,
    decimal ReorderLevel, DateTime LastUpdatedAt);

public record StockItemDetailResponse(
    Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
    Guid WarehouseId, string WarehouseName,
    decimal QuantityOnHand, decimal ReservedQuantity, decimal AverageCostPrice,
    decimal ReorderLevel, DateTime LastUpdatedAt,
    List<StockMovementHistoryItem> History);

public record StockMovementHistoryItem(
    Guid Id, string Type, decimal QuantityIn, decimal QuantityOut,
    decimal BalanceAfter, string? ReferenceNumber, DateTime OccurredAt);

public record UpdateReorderLevelRequest(decimal ReorderLevel);