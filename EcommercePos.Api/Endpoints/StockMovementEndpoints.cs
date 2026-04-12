using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class StockMovementEndpoints
{
    public static void MapStockMovementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-movements").WithTags("StockMovements");

        group.MapGet("/", async (
            [AsParameters] GetStockMovementsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.StockMovements
                .Include(m => m.Product)
                .Include(m => m.MovementTypeCodeNavigation)
                .Include(m => m.FromWarehouse)
                .Include(m => m.ToWarehouse)
                .Where(m => !m.IsDeleted)
                .AsNoTracking();

            if (request.StartDate.HasValue)
                query = query.Where(m => m.OccurredAt >= request.StartDate);

            if (request.EndDate.HasValue)
                query = query.Where(m => m.OccurredAt <= request.EndDate);

            if (!string.IsNullOrWhiteSpace(request.MovementTypeCode))
                query = query.Where(m => m.MovementTypeCode == request.MovementTypeCode);

            if (!string.IsNullOrWhiteSpace(request.WarehouseId))
                query = query.Where(m => m.FromWarehouseId == Guid.Parse(request.WarehouseId) || m.ToWarehouseId == Guid.Parse(request.WarehouseId));

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(m => m.Product.Name.Contains(request.Search) || m.ReferenceNumber.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(m => m.OccurredAt)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new StockMovementResponse(
                    m.Id, m.ProductId, m.Product.Name, m.VariantId,
                    m.MovementTypeCode, m.MovementTypeCodeNavigation.DisplayName,
                    m.FromWarehouseId, m.FromWarehouse.Name,
                    m.ToWarehouseId, m.ToWarehouse.Name,
                    m.QuantityIn, m.QuantityOut, m.BalanceAfter,
                    m.ReferenceType, m.ReferenceNumber, m.OccurredAt))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetStockMovements")
        .WithSummary("Get paginated stock movements");

        group.MapGet("/types", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var types = await context.StockMovementTypes
                .Select(t => new { t.TypeCode, t.DisplayName })
                .ToListAsync(ct);

            return Results.Ok(new { data = types });
        })
        .WithName("GetMovementTypes")
        .WithSummary("Get stock movement types");
    }
}

public record GetStockMovementsRequest(
    int PageIndex = 0, int PageSize = 20, string? Search = null,
    DateTime? StartDate = null, DateTime? EndDate = null,
    string? MovementTypeCode = null, string? WarehouseId = null);

public record StockMovementResponse(
    Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
    string MovementTypeCode, string MovementTypeName,
    Guid? FromWarehouseId, string? FromWarehouseName,
    Guid? ToWarehouseId, string? ToWarehouseName,
    decimal QuantityIn, decimal QuantityOut, decimal BalanceAfter,
    string? ReferenceType, string? ReferenceNumber, DateTime OccurredAt);