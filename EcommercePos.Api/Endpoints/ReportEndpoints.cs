using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports");

        group.MapGet("/dashboard", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var last30Days = today.AddDays(-30);

            var ordersToday = await context.Orders
                .Where(o => !o.IsDeleted && o.OrderDate >= today)
                .CountAsync(ct);

            var ordersThisMonth = await context.Orders
                .Where(o => !o.IsDeleted && o.OrderDate >= startOfMonth)
                .CountAsync(ct);

            var salesToday = await context.Orders
                .Where(o => !o.IsDeleted && o.OrderDate >= today)
                .SumAsync(o => o.TotalAmount, ct);

            var salesThisMonth = await context.Orders
                .Where(o => !o.IsDeleted && o.OrderDate >= startOfMonth)
                .SumAsync(o => o.TotalAmount, ct);

            var totalCustomers = await context.Customers
                .Where(c => !c.IsDeleted)
                .CountAsync(ct);

            var activeCustomers = await context.Customers
                .Where(c => !c.IsDeleted && c.IsActive)
                .CountAsync(ct);

            var totalProducts = await context.Products
                .Where(p => !p.IsDeleted)
                .CountAsync(ct);

            var lowStockProducts = await context.Products
                .Include(p => p.StockItems)
                .Where(p => !p.IsDeleted && p.StockItems.Any(s => s.QuantityOnHand <= p.ReorderLevel))
                .CountAsync(ct);

            var totalWarehouses = await context.Warehouses
                .Where(w => !w.IsDeleted)
                .CountAsync(ct);

            var pendingOrders = await context.Orders
                .Where(o => !o.IsDeleted && o.StatusCode == "PENDING")
                .CountAsync(ct);

            var processingOrders = await context.Orders
                .Where(o => !o.IsDeleted && (o.StatusCode == "CONFIRMED" || o.StatusCode == "PROCESSING"))
                .CountAsync(ct);

            var dailySales = await context.Orders
                .Where(o => !o.IsDeleted && o.OrderDate >= last30Days)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { date = g.Key, sales = g.Sum(o => o.TotalAmount), orders = g.Count() })
                .OrderByDescending(x => x.date)
                .Take(30)
                .ToListAsync(ct);

            var topProducts = await context.OrderItems
                .Include(i => i.Product)
                .Where(i => !i.Order.IsDeleted && i.Order.OrderDate >= last30Days)
                .GroupBy(i => new { i.ProductId, i.Product.Name })
                .Select(g => new { productId = g.Key.ProductId, productName = g.Key.Name, quantity = g.Sum(i => i.Quantity), revenue = g.Sum(i => i.TotalPrice) })
                .OrderByDescending(x => x.revenue)
                .Take(10)
                .ToListAsync(ct);

            var topCategories = await context.OrderItems
                .Include(i => i.Product).ThenInclude(p => p.Category)
                .Where(i => !i.Order.IsDeleted && i.Order.OrderDate >= last30Days)
                .GroupBy(i => i.Product.Category != null ? i.Product.Category.Name : "Uncategorized")
                .Select(g => new { category = g.Key, orders = g.Count(), revenue = g.Sum(i => i.TotalPrice) })
                .OrderByDescending(x => x.revenue)
                .Take(10)
                .ToListAsync(ct);

            return Results.Ok(new { data = new
            {
                summary = new
                {
                    ordersToday,
                    ordersThisMonth,
                    salesToday,
                    salesThisMonth,
                    totalCustomers,
                    activeCustomers,
                    totalProducts,
                    lowStockProducts,
                    totalWarehouses,
                    pendingOrders,
                    processingOrders
                },
                dailySales,
                topProducts,
                topCategories
            }});
        })
        .WithName("GetDashboard")
        .WithSummary("Get dashboard overview");

        group.MapGet("/sales", async (
            [AsParameters] SalesReportRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var startDate = request.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = request.EndDate ?? DateTime.UtcNow;

            var query = context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Warehouse)
                .Where(o => !o.IsDeleted && o.OrderDate >= startDate && o.OrderDate <= endDate);

            if (!string.IsNullOrWhiteSpace(request.WarehouseId))
                query = query.Where(o => o.WarehouseId == Guid.Parse(request.WarehouseId));

            var totalCount = await query.CountAsync(ct);
            var totalSales = await query.SumAsync(o => o.TotalAmount, ct);
            var avgOrderValue = totalCount > 0 ? totalSales / totalCount : 0;

            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new SalesReportItem(
                    o.Id, o.OrderNumber, o.Customer != null ? o.Customer.Phone : null,
                    o.Warehouse != null ? o.Warehouse.Name : null, o.TotalAmount, o.OrderDate, o.StatusCode))
                .ToListAsync(ct);

            return Results.Ok(new { data = new { items, totalCount, totalSales, avgOrderValue } });
        })
        .WithName("GetSalesReport")
        .WithSummary("Get sales report");

        group.MapGet("/inventory", async (
            [AsParameters] InventoryReportRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.StockItems
                .Include(s => s.Product).ThenInclude(p => p.Category)
                .Include(s => s.Warehouse)
                .Where(s => !s.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.WarehouseId))
                query = query.Where(s => s.WarehouseId == Guid.Parse(request.WarehouseId));

            if (!string.IsNullOrWhiteSpace(request.CategoryId))
                query = query.Where(s => s.Product != null && s.Product.CategoryId == Guid.Parse(request.CategoryId));

            if (request.LowStockOnly == true)
                query = query.Where(s => s.Product != null && s.QuantityOnHand <= s.Product.ReorderLevel);

            var totalCount = await query.CountAsync(ct);
            var totalValue = await query.SumAsync(s => s.QuantityOnHand * s.AverageCostPrice, ct);

            var items = await query
                .OrderBy(s => s.Warehouse != null ? s.Warehouse.Name : "")
                .ThenBy(s => s.Product != null ? s.Product.Name : "")
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new InventoryReportItem(
                    s.Id, s.ProductId, s.Product != null ? s.Product.Name : "",
                    s.Product != null ? s.Product.Sku : "",
                    s.WarehouseId, s.Warehouse != null ? s.Warehouse.Name : "",
                    s.QuantityOnHand, s.ReservedQuantity, s.AverageCostPrice,
                    s.Product != null ? s.Product.ReorderLevel : 0,
                    s.Product != null && s.QuantityOnHand <= s.Product.ReorderLevel))
                .ToListAsync(ct);

            return Results.Ok(new { data = new { items, totalCount, totalValue } });
        })
        .WithName("GetInventoryReport")
        .WithSummary("Get inventory report");
    }
}

public record SalesReportRequest(
    int PageIndex = 0, int PageSize = 20,
    DateTime? StartDate = null, DateTime? EndDate = null,
    string? WarehouseId = null);

public record SalesReportItem(
    Guid Id, string OrderNumber, string? CustomerPhone,
    string? Warehouse, decimal TotalAmount, DateTime OrderDate, string Status);

public record InventoryReportRequest(
    int PageIndex = 0, int PageSize = 20,
    string? WarehouseId = null, string? CategoryId = null,
    bool? LowStockOnly = null);

public record InventoryReportItem(
    Guid Id, Guid ProductId, string ProductName, string? Sku,
    Guid? WarehouseId, string? WarehouseName,
    decimal QuantityOnHand, decimal ReservedQuantity, decimal UnitCost,
    decimal ReorderLevel, bool IsLowStock);