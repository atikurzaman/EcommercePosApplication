using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class GetStockItems
{
    public sealed record Request(
        int PageIndex = 0,
        int PageSize = 10,
        string? Search = null,
        Guid? WarehouseId = null,
        Guid? ProductId = null,
        Guid? CategoryId = null,
        bool? LowStock = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public Guid WarehouseId { get; init; }
        public string WarehouseName { get; init; } = string.Empty;
        public decimal QuantityOnHand { get; init; }
        public decimal ReservedQuantity { get; init; }
        public decimal AvailableQuantity { get; init; }
        public decimal AverageCostPrice { get; init; }
        public decimal? ReorderLevel { get; init; }
        public bool IsLowStock { get; init; }
    }

    public sealed record Query(
        int PageIndex,
        int PageSize,
        string? Search,
        Guid? WarehouseId,
        Guid? ProductId,
        Guid? CategoryId,
        bool? LowStock);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.StockItems
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Where(s => !s.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(s => s.Product.Name.Contains(query.Search) || s.Product.Sku.Contains(query.Search));
            }

            if (query.WarehouseId.HasValue)
            {
                dbQuery = dbQuery.Where(s => s.WarehouseId == query.WarehouseId.Value);
            }

            if (query.ProductId.HasValue)
            {
                dbQuery = dbQuery.Where(s => s.ProductId == query.ProductId.Value);
            }

            if (query.CategoryId.HasValue)
            {
                dbQuery = dbQuery.Where(s => s.Product.CategoryId == query.CategoryId.Value);
            }

            if (query.LowStock == true)
            {
                dbQuery = dbQuery.Where(s => s.ReorderLevel.HasValue && s.QuantityOnHand <= s.ReorderLevel.Value);
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(s => s.Product.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(s => new Response
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    ProductName = s.Product.Name,
                    Sku = s.Product.Sku,
                    WarehouseId = s.WarehouseId,
                    WarehouseName = s.Warehouse.Name,
                    QuantityOnHand = s.QuantityOnHand,
                    ReservedQuantity = s.ReservedQuantity,
                    AvailableQuantity = s.QuantityOnHand - s.ReservedQuantity,
                    AverageCostPrice = s.AverageCostPrice,
                    ReorderLevel = s.ReorderLevel,
                    IsLowStock = s.ReorderLevel.HasValue && s.QuantityOnHand <= s.ReorderLevel.Value
                })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
