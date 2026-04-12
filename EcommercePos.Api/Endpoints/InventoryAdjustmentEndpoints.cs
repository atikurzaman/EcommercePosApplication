using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class InventoryAdjustmentEndpoints
{
    public static void MapInventoryAdjustmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory-adjustments").WithTags("InventoryAdjustments");

        group.MapGet("/", async (
            [AsParameters] GetInventoryAdjustmentsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.InventoryAdjustments
                .Include(a => a.Warehouse)
                .Include(a => a.CreatedByNavigation)
                .Where(a => !a.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.WarehouseId))
                query = query.Where(a => a.WarehouseId == Guid.Parse(request.WarehouseId));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(a => a.AdjustmentDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new InventoryAdjustmentResponse(
                    a.Id, a.AdjustmentNo, a.WarehouseId, a.Warehouse.Name,
                    a.AdjustmentDate, a.AdjustmentType, a.Reason,
                    a.ApprovedByUserId != null, a.ApprovedAt, a.CreatedAt,
                    a.CreatedByNavigation != null ? a.CreatedByNavigation.FirstName + " " + a.CreatedByNavigation.LastName : null))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetInventoryAdjustments")
        .WithSummary("Get paginated inventory adjustments");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var adj = await context.InventoryAdjustments
                .Include(a => a.Warehouse)
                .Include(a => a.InventoryAdjustmentLines)
                .ThenInclude(l => l.Product)
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (adj == null)
                return Results.NotFound(new { error = "Inventory adjustment not found" });

            var response = new InventoryAdjustmentDetailResponse(
                adj.Id, adj.AdjustmentNo, adj.WarehouseId, adj.Warehouse.Name,
                adj.AdjustmentDate, adj.AdjustmentType, adj.Reason, adj.Notes,
                adj.ApprovedByUserId != null, adj.ApprovedAt, adj.CreatedAt,
                adj.InventoryAdjustmentLines.Select(l => new InventoryAdjustmentLineResponse(
                    l.Id, l.ProductId, l.Product.Name, l.VariantId,
                    l.AdjustmentQuantity, l.Remarks)).ToList());

            return Results.Ok(new { data = response });
        })
        .WithName("GetInventoryAdjustmentById")
        .WithSummary("Get inventory adjustment with lines");

        group.MapPost("/", async (CreateInventoryAdjustmentRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var adjNo = $"ADJ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            
            var adj = new InventoryAdjustments
            {
                Id = Guid.NewGuid(),
                AdjustmentNo = adjNo,
                WarehouseId = request.WarehouseId,
                AdjustmentDate = DateTime.UtcNow,
                AdjustmentType = request.AdjustmentType,
                Reason = request.Reason,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.InventoryAdjustments.Add(adj);

            foreach (var line in request.Lines)
            {
                var adjLine = new InventoryAdjustmentLines
                {
                    Id = Guid.NewGuid(),
                    InventoryAdjustmentId = adj.Id,
                    ProductId = line.ProductId,
                    VariantId = line.VariantId,
                    AdjustmentQuantity = line.QuantityAdjusted,
                    Remarks = line.Reason
                };
                context.InventoryAdjustmentLines.Add(adjLine);
            }

            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/inventory-adjustments/{adj.Id}", new { data = new { adj.Id, adj.AdjustmentNo } });
        })
        .WithName("CreateInventoryAdjustment")
        .WithSummary("Create inventory adjustment");

        group.MapPost("/{id:guid}/approve", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var adj = await context.InventoryAdjustments
                .Include(a => a.InventoryAdjustmentLines)
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (adj == null)
                return Results.NotFound(new { error = "Inventory adjustment not found" });

            if (adj.ApprovedByUserId != null)
                return Results.BadRequest(new { error = "Adjustment already approved" });

            adj.ApprovedByUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            adj.ApprovedAt = DateTime.UtcNow;
            adj.UpdatedAt = DateTime.UtcNow;

            foreach (var line in adj.InventoryAdjustmentLines)
            {
                var stockItem = await context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == line.ProductId && s.WarehouseId == adj.WarehouseId && !s.IsDeleted, ct);

                if (stockItem != null)
                {
                    stockItem.QuantityOnHand += line.AdjustmentQuantity;
                    stockItem.UpdatedAt = DateTime.UtcNow;

                    context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        StockItemId = stockItem.Id,
                        MovementTypeCode = adj.AdjustmentType == "INCREASE" ? "ADJ_IN" : "ADJ_OUT",
                        QuantityIn = adj.AdjustmentType == "INCREASE" ? line.AdjustmentQuantity : 0,
                        QuantityOut = adj.AdjustmentType == "DECREASE" ? line.AdjustmentQuantity : 0,
                        BalanceAfter = stockItem.QuantityOnHand,
                        ReferenceType = "InventoryAdjustment",
                        ReferenceId = adj.Id,
                        ReferenceNumber = adj.AdjustmentNo,
                        Notes = line.Remarks,
                        OccurredAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { adj.Id, adj.ApprovedAt } });
        })
        .WithName("ApproveInventoryAdjustment")
        .WithSummary("Approve inventory adjustment");
    }
}

public record GetInventoryAdjustmentsRequest(
    int PageIndex = 0, int PageSize = 10, string? WarehouseId = null);

public record InventoryAdjustmentResponse(
    Guid Id, string AdjustmentNo, Guid WarehouseId, string WarehouseName,
    DateTime AdjustmentDate, string AdjustmentType, string Reason,
    bool IsApproved, DateTime? ApprovedAt, DateTime CreatedAt, string? CreatedBy);

public record InventoryAdjustmentDetailResponse(
    Guid Id, string AdjustmentNo, Guid WarehouseId, string WarehouseName,
    DateTime AdjustmentDate, string AdjustmentType, string Reason, string? Notes,
    bool IsApproved, DateTime? ApprovedAt, DateTime CreatedAt,
    List<InventoryAdjustmentLineResponse> Lines);

public record InventoryAdjustmentLineResponse(
    Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
    decimal QuantityAdjusted, string Reason);

public record CreateInventoryAdjustmentRequest(
    Guid WarehouseId, string AdjustmentType, string Reason, string? Notes,
    List<CreateInventoryAdjustmentLineRequest> Lines);

public record CreateInventoryAdjustmentLineRequest(
    Guid ProductId, Guid? VariantId, decimal QuantityAdjusted, string Reason);