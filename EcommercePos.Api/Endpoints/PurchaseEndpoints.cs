using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class PurchaseEndpoints
{
    public static void MapPurchaseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/purchases").WithTags("Purchases");

        group.MapGet("/", async (
            [AsParameters] GetPurchasesRequest request, 
            ApplicationDbContext context, 
            CancellationToken ct) =>
        {
            var query = context.PurchaseOrders
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(p => p.OrderNumber.Contains(request.Search));
            }

            if (request.Status != null)
            {
                query = query.Where(p => p.Status == request.Status);
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(p => p.OrderDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PurchaseResponse(
                    p.Id, p.OrderNumber, p.OrderDate, p.GrandTotal, p.Status, 
                    p.SupplierId, p.WarehouseId))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetPurchases")
        .WithSummary("Get paginated purchases");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var purchase = await context.PurchaseOrders
                .Include(p => p.PurchaseOrderLines)
                .Where(p => p.Id == id && !p.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (purchase == null)
                return Results.NotFound(new { error = "Purchase not found" });

            return Results.Ok(new { data = new PurchaseDetailResponse(
                purchase.Id, purchase.OrderNumber, purchase.OrderDate, purchase.SubTotal,
                purchase.DiscountAmount, purchase.TotalTaxAmount, purchase.GrandTotal, purchase.Status,
                purchase.SupplierId, purchase.WarehouseId, purchase.CreatedBy) });
        })
        .WithName("GetPurchaseById")
        .WithSummary("Get purchase by id");

        group.MapPost("/", async (CreatePurchaseRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var orderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            var purchase = new PurchaseOrders
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                SubTotal = request.SubTotal,
                DiscountAmount = request.DiscountAmount,
                TotalTaxAmount = request.TotalTaxAmount,
                GrandTotal = request.GrandTotal,
                Status = "PENDING",
                SupplierId = request.SupplierId,
                WarehouseId = request.WarehouseId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.PurchaseOrders.Add(purchase);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/purchases/{purchase.Id}", new { data = new PurchaseResponse(
                purchase.Id, purchase.OrderNumber, purchase.OrderDate, purchase.GrandTotal,
                purchase.Status, purchase.SupplierId, purchase.WarehouseId) });
        })
        .WithName("CreatePurchase")
        .WithSummary("Create a new purchase");

        group.MapPut("/{id:guid}", async (Guid id, UpdatePurchaseRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var purchase = await context.PurchaseOrders.FindAsync(new object[] { id }, ct);
            if (purchase == null || purchase.IsDeleted)
                return Results.NotFound(new { error = "Purchase not found" });

            purchase.Status = request.Status;
            purchase.SubTotal = request.SubTotal;
            purchase.DiscountAmount = request.DiscountAmount;
            purchase.TotalTaxAmount = request.TotalTaxAmount;
            purchase.GrandTotal = request.GrandTotal;
            purchase.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new PurchaseResponse(
                purchase.Id, purchase.OrderNumber, purchase.OrderDate, purchase.GrandTotal,
                purchase.Status, purchase.SupplierId, purchase.WarehouseId) });
        })
        .WithName("UpdatePurchase")
        .WithSummary("Update an existing purchase");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var purchase = await context.PurchaseOrders.FindAsync(new object[] { id }, ct);
            if (purchase == null || purchase.IsDeleted)
                return Results.NotFound(new { error = "Purchase not found" });

            purchase.IsDeleted = true;
            purchase.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeletePurchase")
        .WithSummary("Soft delete a purchase");
    }
}

public record GetPurchasesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null, string? Status = null);
public record PurchaseResponse(
    Guid Id, string OrderNumber, DateTime OrderDate, decimal GrandTotal, string Status,
    Guid SupplierId, Guid? WarehouseId);
public record PurchaseDetailResponse(
    Guid Id, string OrderNumber, DateTime OrderDate, decimal SubTotal, decimal DiscountAmount,
    decimal TotalTaxAmount, decimal GrandTotal, string Status, Guid SupplierId,
    Guid? WarehouseId, Guid? CreatedBy);
public record CreatePurchaseRequest(
    decimal SubTotal, decimal DiscountAmount, decimal TotalTaxAmount, decimal GrandTotal,
    Guid SupplierId, Guid? WarehouseId);
public record UpdatePurchaseRequest(
    string Status, decimal SubTotal, decimal DiscountAmount, decimal TotalTaxAmount, decimal GrandTotal);
