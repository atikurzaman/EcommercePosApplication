using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class SaleEndpoints
{
    public static void MapSaleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sales").WithTags("Sales");

        group.MapGet("/", async (
            [AsParameters] GetSalesRequest request, 
            ApplicationDbContext context, 
            CancellationToken ct) =>
        {
            var query = context.Orders
                .Where(o => !o.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(o => o.OrderNumber.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new SaleResponse(
                    o.Id, o.OrderNumber, o.OrderDate, o.TotalAmount, o.PaidAmount, 
                    o.StatusCode, o.TotalAmount - o.PaidAmount > 0 ? "PARTIAL" : "PAID"))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetSales")
        .WithSummary("Get paginated sales");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Id == id && !o.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (order == null)
                return Results.NotFound(new { error = "Sale not found" });

            return Results.Ok(new { data = new SaleDetailResponse(
                order.Id, order.OrderNumber, order.OrderDate, order.SubTotal, order.DiscountAmount,
                order.TaxAmount, order.TotalAmount, order.PaidAmount, order.TotalAmount - order.PaidAmount,
                order.StatusCode, order.ShippingAmount, order.CustomerId,
                order.WarehouseId, order.CreatedBy) });
        })
        .WithName("GetSaleById")
        .WithSummary("Get sale by id");

        group.MapPost("/", async (CreateSaleRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var orderNumber = $"SL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            
            var order = new Orders
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                SubTotal = request.SubTotal,
                DiscountAmount = request.DiscountAmount,
                TaxAmount = request.TaxAmount,
                TotalAmount = request.TotalAmount,
                PaidAmount = request.PaidAmount,
                StatusCode = "PENDING",
                CustomerId = request.CustomerId,
                WarehouseId = request.WarehouseId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/sales/{order.Id}", new { data = new SaleResponse(
                order.Id, order.OrderNumber, order.OrderDate, order.TotalAmount, order.PaidAmount,
                order.StatusCode, order.TotalAmount - order.PaidAmount > 0 ? "PARTIAL" : "PAID") });
        })
        .WithName("CreateSale")
        .WithSummary("Create a new sale");

        group.MapPut("/{id:guid}", async (Guid id, UpdateSaleRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var order = await context.Orders.FindAsync(new object[] { id }, ct);
            if (order == null || order.IsDeleted)
                return Results.NotFound(new { error = "Sale not found" });

            order.SubTotal = request.SubTotal;
            order.DiscountAmount = request.DiscountAmount;
            order.TaxAmount = request.TaxAmount;
            order.TotalAmount = request.TotalAmount;
            order.PaidAmount = request.PaidAmount;
            order.StatusCode = request.StatusCode;
            order.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new SaleResponse(
                order.Id, order.OrderNumber, order.OrderDate, order.TotalAmount, order.PaidAmount,
                order.StatusCode, order.TotalAmount - order.PaidAmount > 0 ? "PARTIAL" : "PAID") });
        })
        .WithName("UpdateSale")
        .WithSummary("Update an existing sale");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var order = await context.Orders.FindAsync(new object[] { id }, ct);
            if (order == null || order.IsDeleted)
                return Results.NotFound(new { error = "Sale not found" });

            order.IsDeleted = true;
            order.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteSale")
        .WithSummary("Soft delete a sale");
    }
}

public record GetSalesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null, string? Status = null);
public record SaleResponse(
    Guid Id, string OrderNumber, DateTime OrderDate, decimal TotalAmount, decimal PaidAmount,
    string StatusCode, string PaymentStatus);
public record SaleDetailResponse(
    Guid Id, string OrderNumber, DateTime OrderDate, decimal SubTotal, decimal DiscountAmount,
    decimal TaxAmount, decimal TotalAmount, decimal PaidAmount, decimal DueAmount,
    string StatusCode, decimal ShippingAmount, Guid CustomerId,
    Guid? WarehouseId, Guid? CreatedBy);
public record CreateSaleRequest(
    decimal SubTotal, decimal DiscountAmount, decimal TaxAmount, decimal TotalAmount,
    decimal PaidAmount, Guid CustomerId, Guid? WarehouseId);
public record UpdateSaleRequest(
    decimal SubTotal, decimal DiscountAmount, decimal TaxAmount, decimal TotalAmount,
    decimal PaidAmount, string StatusCode);
