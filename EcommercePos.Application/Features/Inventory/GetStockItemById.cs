using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class GetStockItemById
{
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
        public DateTime? LastCountDate { get; init; }
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.StockItems
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Where(s => s.Id == query.Id && !s.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result<Response>.Failure(Error.NotFound("Stock item not found"));

            var response = new Response
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Sku = item.Product.Sku,
                WarehouseId = item.WarehouseId,
                WarehouseName = item.Warehouse.Name,
                QuantityOnHand = item.QuantityOnHand,
                ReservedQuantity = item.ReservedQuantity,
                AvailableQuantity = item.QuantityOnHand - item.ReservedQuantity,
                AverageCostPrice = item.AverageCostPrice,
                ReorderLevel = item.ReorderLevel,
                LastCountDate = item.LastCountDate
            };

            return Result<Response>.Success(response);
        }
    }
}
