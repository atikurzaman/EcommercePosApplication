using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class GetLowStockItems
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public string WarehouseName { get; init; } = string.Empty;
        public decimal QuantityOnHand { get; init; }
        public decimal ReorderLevel { get; init; }
    }

    public sealed record Query(Guid? WarehouseId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.StockItems
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Where(s => !s.IsDeleted && s.ReorderLevel.HasValue && s.QuantityOnHand <= s.ReorderLevel.Value)
                .AsNoTracking();

            if (query.WarehouseId.HasValue)
            {
                dbQuery = dbQuery.Where(s => s.WarehouseId == query.WarehouseId.Value);
            }

            var items = await dbQuery
                .OrderBy(s => s.QuantityOnHand)
                .Select(s => new Response
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    ProductName = s.Product.Name,
                    Sku = s.Product.Sku,
                    WarehouseName = s.Warehouse.Name,
                    QuantityOnHand = s.QuantityOnHand,
                    ReorderLevel = s.ReorderLevel ?? 0
                })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
