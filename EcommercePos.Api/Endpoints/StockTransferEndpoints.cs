using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class StockTransferEndpoints
{
    public static void MapStockTransferEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock-transfers").WithTags("StockTransfers");

        group.MapGet("/", async (
            [AsParameters] GetStockTransfersRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.StockTransfers
                .Include(t => t.FromWarehouse)
                .Include(t => t.ToWarehouse)
                .Include(t => t.CreatedByNavigation)
                .Where(t => !t.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.FromWarehouseId))
                query = query.Where(t => t.FromWarehouseId == Guid.Parse(request.FromWarehouseId));

            if (!string.IsNullOrWhiteSpace(request.ToWarehouseId))
                query = query.Where(t => t.ToWarehouseId == Guid.Parse(request.ToWarehouseId));

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(t => t.Status == request.Status);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(t => t.TransferDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new StockTransferResponse(
                    t.Id, t.TransferNo, t.FromWarehouseId, t.FromWarehouse.Name,
                    t.ToWarehouseId, t.ToWarehouse.Name, t.TransferDate, t.Status,
                    t.CreatedAt, t.CreatedByNavigation != null ? t.CreatedByNavigation.FirstName + " " + t.CreatedByNavigation.LastName : null))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetStockTransfers")
        .WithSummary("Get paginated stock transfers");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var transfer = await context.StockTransfers
                .Include(t => t.FromWarehouse)
                .Include(t => t.ToWarehouse)
                .Include(t => t.StockTransferLines)
                .ThenInclude(l => l.Product)
                .Where(t => t.Id == id && !t.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (transfer == null)
                return Results.NotFound(new { error = "Stock transfer not found" });

            var response = new StockTransferDetailResponse(
                transfer.Id, transfer.TransferNo,
                transfer.FromWarehouseId, transfer.FromWarehouse.Name,
                transfer.ToWarehouseId, transfer.ToWarehouse.Name,
                transfer.TransferDate, transfer.Status, transfer.Notes,
                transfer.CreatedAt,
                transfer.StockTransferLines.Select(l => new StockTransferLineResponse(
                    l.Id, l.ProductId, l.Product.Name, l.VariantId,
                    l.Quantity)).ToList());

            return Results.Ok(new { data = response });
        })
        .WithName("GetStockTransferById")
        .WithSummary("Get stock transfer with lines");

        group.MapPost("/", async (CreateStockTransferRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var transferNo = $"TRF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            var transfer = new StockTransfers
            {
                Id = Guid.NewGuid(),
                TransferNo = transferNo,
                FromWarehouseId = request.FromWarehouseId,
                ToWarehouseId = request.ToWarehouseId,
                TransferDate = DateTime.UtcNow,
                Status = "PENDING",
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.StockTransfers.Add(transfer);

            foreach (var line in request.Lines)
            {
                var transferLine = new StockTransferLines
                {
                    Id = Guid.NewGuid(),
                    TransferId = transfer.Id,
                    ProductId = line.ProductId,
                    VariantId = line.VariantId,
                    Quantity = line.Quantity
                };
                context.StockTransferLines.Add(transferLine);

                var stockItem = await context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == line.ProductId && s.WarehouseId == request.FromWarehouseId && !s.IsDeleted, ct);

                if (stockItem != null && stockItem.QuantityOnHand >= line.Quantity)
                {
                    stockItem.QuantityOnHand -= line.Quantity;
                    stockItem.UpdatedAt = DateTime.UtcNow;

                    context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        StockItemId = stockItem.Id,
                        MovementTypeCode = "TRANSFER_OUT",
                        QuantityIn = 0,
                        QuantityOut = line.Quantity,
                        BalanceAfter = stockItem.QuantityOnHand,
                        ReferenceType = "StockTransfer",
                        ReferenceId = transfer.Id,
                        ReferenceNumber = transferNo,
                        OccurredAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/stock-transfers/{transfer.Id}", new { data = new { transfer.Id, transfer.TransferNo } });
        })
        .WithName("CreateStockTransfer")
        .WithSummary("Create stock transfer");

        group.MapPost("/{id:guid}/receive", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var transfer = await context.StockTransfers
                .Include(t => t.StockTransferLines)
                .Where(t => t.Id == id && !t.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (transfer == null)
                return Results.NotFound(new { error = "Stock transfer not found" });

            if (transfer.Status == "RECEIVED")
                return Results.BadRequest(new { error = "Transfer already received" });

            foreach (var line in transfer.StockTransferLines)
            {
                var toStockItem = await context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == line.ProductId && s.WarehouseId == transfer.ToWarehouseId && !s.IsDeleted, ct);

                if (toStockItem == null)
                {
                    toStockItem = new StockItems
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        WarehouseId = transfer.ToWarehouseId,
                        QuantityOnHand = 0,
                        ReservedQuantity = 0,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.StockItems.Add(toStockItem);
                }

                toStockItem.QuantityOnHand += line.Quantity;
                toStockItem.UpdatedAt = DateTime.UtcNow;

                context.StockMovements.Add(new StockMovements
                {
                    Id = Guid.NewGuid(),
                    ProductId = line.ProductId,
                    VariantId = line.VariantId,
                    StockItemId = toStockItem.Id,
                    MovementTypeCode = "TRANSFER_IN",
                    QuantityIn = line.Quantity,
                    QuantityOut = 0,
                    BalanceAfter = toStockItem.QuantityOnHand,
                    ReferenceType = "StockTransfer",
                    ReferenceId = transfer.Id,
                    ReferenceNumber = transfer.TransferNo,
                    OccurredAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            transfer.Status = "RECEIVED";
            transfer.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { transfer.Id, transfer.Status } });
        })
        .WithName("ReceiveStockTransfer")
        .WithSummary("Receive stock transfer");
    }
}

public record GetStockTransfersRequest(
    int PageIndex = 0, int PageSize = 10, string? FromWarehouseId = null,
    string? ToWarehouseId = null, string? Status = null);

public record StockTransferResponse(
    Guid Id, string TransferNo, Guid FromWarehouseId, string FromWarehouseName,
    Guid ToWarehouseId, string ToWarehouseName, DateTime TransferDate, string Status,
    DateTime CreatedAt, string? CreatedBy);

public record StockTransferDetailResponse(
    Guid Id, string TransferNo,
    Guid FromWarehouseId, string FromWarehouseName,
    Guid ToWarehouseId, string ToWarehouseName,
    DateTime TransferDate, string Status, string? Notes, DateTime CreatedAt,
    List<StockTransferLineResponse> Lines);

public record StockTransferLineResponse(
    Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
    decimal Quantity);

public record CreateStockTransferRequest(
    Guid FromWarehouseId, Guid ToWarehouseId, string? Notes,
    List<CreateStockTransferLineRequest> Lines);

public record CreateStockTransferLineRequest(
    Guid ProductId, Guid? VariantId, decimal Quantity, decimal UnitCost);

public record ReceiveStockTransferRequest(List<ReceiveStockTransferLineRequest> Lines);

public record ReceiveStockTransferLineRequest(Guid LineId, decimal QuantityReceived);