using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class GetStockItems
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null, Guid? WarehouseId = null, Guid? ProductId = null, bool? LowStock = null);

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

    public sealed record Query(int PageIndex, int PageSize, string? Search, Guid? WarehouseId, Guid? ProductId, bool? LowStock);

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
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
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

public static class GetStockMovements
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid StockItemId { get; init; }
        public string MovementType { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
        public decimal QuantityBefore { get; init; }
        public decimal QuantityAfter { get; init; }
        public string? ReferenceNo { get; init; }
        public string? Notes { get; init; }
        public Guid? CreatedBy { get; init; }
        public string? CreatedByName { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(Guid StockItemId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var movements = await _context.StockMovements
                .Include(m => m.CreatedByNavigation)
                .Where(m => m.StockItemId == query.StockItemId)
                .OrderByDescending(m => m.CreatedAt)
                .AsNoTracking()
                .Select(m => new Response
                {
                    Id = m.Id,
                    StockItemId = m.StockItemId ?? Guid.Empty,
                    MovementType = m.MovementTypeCode,
                    Quantity = m.QuantityIn,
                    QuantityBefore = m.BalanceAfter - m.QuantityIn,
                    QuantityAfter = m.BalanceAfter,
                    ReferenceNo = m.ReferenceNumber,
                    Notes = m.Notes,
                    CreatedBy = m.CreatedBy,
                    CreatedByName = m.CreatedByNavigation != null ? m.CreatedByNavigation.UserName : null,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(movements);
        }
    }
}

public static class CreateStockAdjustment
{
    public sealed record Request
    {
        public Guid StockItemId { get; init; }
        public decimal Quantity { get; init; }
        public string AdjustmentType { get; init; } = string.Empty;
        public string? Reason { get; init; }
        public string? Notes { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public decimal NewQuantity { get; init; }
    }

    public sealed record Command(Request Request, Guid UserId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var stockItem = await _context.StockItems
                .Where(s => s.Id == command.Request.StockItemId && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (stockItem == null)
                return Result<Response>.Failure(Error.NotFound("Stock item not found"));

            var qtyBefore = stockItem.QuantityOnHand;
            decimal qtyChange = 0;

            switch (command.Request.AdjustmentType.ToUpper())
            {
                case "ADD":
                    qtyChange = command.Request.Quantity;
                    stockItem.QuantityOnHand += command.Request.Quantity;
                    break;
                case "REMOVE":
                    qtyChange = -command.Request.Quantity;
                    if (stockItem.QuantityOnHand < command.Request.Quantity)
                        return Result<Response>.Failure(Error.Conflict("Insufficient stock"));
                    stockItem.QuantityOnHand -= command.Request.Quantity;
                    break;
                case "SET":
                    qtyChange = command.Request.Quantity - stockItem.QuantityOnHand;
                    stockItem.QuantityOnHand = command.Request.Quantity;
                    break;
                default:
                    return Result<Response>.Failure(Error.BadRequest("Invalid adjustment type"));
            }

            var movement = new StockMovements
            {
                Id = Guid.NewGuid(),
                StockItemId = stockItem.Id,
                ProductId = stockItem.ProductId,
                MovementTypeCode = command.Request.AdjustmentType.ToUpper(),
                QuantityIn = command.Request.AdjustmentType.ToUpper() == "ADD" ? Math.Abs(qtyChange) : 0,
                QuantityOut = command.Request.AdjustmentType.ToUpper() == "REMOVE" ? Math.Abs(qtyChange) : 0,
                BalanceAfter = stockItem.QuantityOnHand,
                ReferenceNumber = command.Request.Reason,
                Notes = command.Request.Notes,
                CreatedBy = command.UserId,
                CreatedAt = DateTime.Now
            };

            stockItem.LastUpdatedAt = DateTime.Now;
            _context.StockMovements.Add(movement);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = movement.Id,
                NewQuantity = stockItem.QuantityOnHand
            });
        }
    }
}

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
