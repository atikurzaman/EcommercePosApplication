using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Report;

public static class GetDashboardReport
{
    public sealed record DailySalesItem(DateTime Date, decimal Sales, int Orders);
    public sealed record TopProductItem(Guid ProductId, string ProductName, decimal Quantity, decimal Revenue);
    public sealed record TopCategoryItem(string Category, int Orders, decimal Revenue);

    public sealed record Summary(
        int OrdersToday,
        int OrdersThisMonth,
        decimal SalesToday,
        decimal SalesThisMonth,
        int TotalCustomers,
        int ActiveCustomers,
        int TotalProducts,
        int LowStockProducts,
        int TotalWarehouses,
        int PendingOrders,
        int ProcessingOrders);

    public sealed record Response(
        Summary Summary,
        List<DailySalesItem> DailySales,
        List<TopProductItem> TopProducts,
        List<TopCategoryItem> TopCategories);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var last30Days = today.AddDays(-30);

            var summary = new Summary(
                await _context.Orders.Where(o => !o.IsDeleted && o.OrderDate >= today).CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.OrderDate >= startOfMonth).CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.OrderDate >= today).SumAsync(o => o.TotalAmount, ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.OrderDate >= startOfMonth).SumAsync(o => o.TotalAmount, ct),
                await _context.Customers.Where(c => !c.IsDeleted).CountAsync(ct),
                await _context.Customers.Where(c => !c.IsDeleted && c.IsActive).CountAsync(ct),
                await _context.Products.Where(p => !p.IsDeleted).CountAsync(ct),
                await _context.Products.Include(p => p.StockItems).Where(p => !p.IsDeleted && p.StockItems.Any(s => s.QuantityOnHand <= p.ReorderLevel)).CountAsync(ct),
                await _context.Warehouses.Where(w => !w.IsDeleted).CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "PENDING").CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && (o.StatusCode == "CONFIRMED" || o.StatusCode == "PROCESSING")).CountAsync(ct));

            var dailySales = await _context.Orders
                .Where(o => !o.IsDeleted && o.OrderDate >= last30Days)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailySalesItem(g.Key, g.Sum(o => o.TotalAmount), g.Count()))
                .OrderByDescending(x => x.Date)
                .Take(30)
                .ToListAsync(ct);

            var topProducts = await _context.OrderItems
                .Include(i => i.Product)
                .Where(i => !i.Order.IsDeleted && i.Order.OrderDate >= last30Days)
                .GroupBy(i => new { i.ProductId, i.Product.Name })
                .Select(g => new TopProductItem(g.Key.ProductId, g.Key.Name, g.Sum(i => i.Quantity), g.Sum(i => i.TotalPrice)))
                .OrderByDescending(x => x.Revenue)
                .Take(10)
                .ToListAsync(ct);

            var topCategories = await _context.OrderItems
                .Include(i => i.Product).ThenInclude(p => p.Category)
                .Where(i => !i.Order.IsDeleted && i.Order.OrderDate >= last30Days)
                .GroupBy(i => i.Product.Category != null ? i.Product.Category.Name : "Uncategorized")
                .Select(g => new TopCategoryItem(g.Key, g.Count(), g.Sum(i => i.TotalPrice)))
                .OrderByDescending(x => x.Revenue)
                .Take(10)
                .ToListAsync(ct);

            return Result<Response>.Success(new Response(summary, dailySales, topProducts, topCategories));
        }
    }
}

public static class GetSalesReport
{
    public sealed record Query(
        int PageIndex = 0,
        int PageSize = 20,
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        Guid? WarehouseId = null);

    public sealed record Item(
        Guid Id, string OrderNumber, string? CustomerPhone,
        string? Warehouse, decimal TotalAmount, DateTime OrderDate, string Status);

    public sealed record Response(List<Item> Items, int TotalCount, decimal TotalSales, decimal AvgOrderValue);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var startDate = query.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = query.EndDate ?? DateTime.UtcNow;

            var dbQuery = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Warehouse)
                .Where(o => !o.IsDeleted && o.OrderDate >= startDate && o.OrderDate <= endDate);

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(o => o.WarehouseId == query.WarehouseId.Value);

            var totalCount = await dbQuery.CountAsync(ct);
            var totalSales = await dbQuery.SumAsync(o => o.TotalAmount, ct);
            var avgOrderValue = totalCount > 0 ? totalSales / totalCount : 0;

            var items = await dbQuery
                .OrderByDescending(o => o.OrderDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(o => new Item(
                    o.Id,
                    o.OrderNumber,
                    o.Customer != null ? o.Customer.Phone : null,
                    o.Warehouse != null ? o.Warehouse.Name : null,
                    o.TotalAmount,
                    o.OrderDate,
                    o.StatusCode))
                .ToListAsync(ct);

            return Result<Response>.Success(new Response(items, totalCount, totalSales, avgOrderValue));
        }
    }
}

public static class GetInventoryReport
{
    public sealed record Query(
        int PageIndex = 0,
        int PageSize = 20,
        Guid? WarehouseId = null,
        Guid? CategoryId = null,
        bool? LowStockOnly = null);

    public sealed record Item(
        Guid Id, Guid ProductId, string ProductName, string? Sku,
        Guid? WarehouseId, string? WarehouseName,
        decimal QuantityOnHand, decimal ReservedQuantity, decimal UnitCost,
        decimal ReorderLevel, bool IsLowStock);

    public sealed record Response(List<Item> Items, int TotalCount, decimal TotalValue);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.StockItems
                .Include(s => s.Product).ThenInclude(p => p.Category)
                .Include(s => s.Warehouse)
                .Where(s => !s.IsDeleted);

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(s => s.WarehouseId == query.WarehouseId.Value);

            if (query.CategoryId.HasValue)
                dbQuery = dbQuery.Where(s => s.Product != null && s.Product.CategoryId == query.CategoryId.Value);

            if (query.LowStockOnly == true)
                dbQuery = dbQuery.Where(s => s.Product != null && s.QuantityOnHand <= s.Product.ReorderLevel);

            var totalCount = await dbQuery.CountAsync(ct);
            var totalValue = await dbQuery.SumAsync(s => s.QuantityOnHand * s.AverageCostPrice, ct);

            var items = await dbQuery
                .OrderBy(s => s.Warehouse != null ? s.Warehouse.Name : string.Empty)
                .ThenBy(s => s.Product != null ? s.Product.Name : string.Empty)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(s => new Item(
                    s.Id,
                    s.ProductId,
                    s.Product != null ? s.Product.Name : string.Empty,
                    s.Product != null ? s.Product.Sku : string.Empty,
                    s.WarehouseId,
                    s.Warehouse != null ? s.Warehouse.Name : string.Empty,
                    s.QuantityOnHand,
                    s.ReservedQuantity,
                    s.AverageCostPrice,
                    s.Product != null ? s.Product.ReorderLevel : 0,
                    s.Product != null && s.QuantityOnHand <= s.Product.ReorderLevel))
                .ToListAsync(ct);

            return Result<Response>.Success(new Response(items, totalCount, totalValue));
        }
    }
}
