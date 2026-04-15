using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class GetAllStockMovements
{
    public sealed record Query(
        int PageIndex = 0, int PageSize = 20, string? Search = null,
        DateTime? StartDate = null, DateTime? EndDate = null,
        string? MovementTypeCode = null, Guid? WarehouseId = null);

    public sealed record Response(
        Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
        string MovementTypeCode, string MovementTypeName,
        Guid? FromWarehouseId, string? FromWarehouseName,
        Guid? ToWarehouseId, string? ToWarehouseName,
        decimal QuantityIn, decimal QuantityOut, decimal BalanceAfter,
        string? ReferenceType, string? ReferenceNumber, DateTime OccurredAt);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.StockMovements
                .Include(m => m.Product)
                .Include(m => m.MovementTypeCodeNavigation)
                .Include(m => m.FromWarehouse)
                .Include(m => m.ToWarehouse)
                .Where(m => !m.IsDeleted)
                .AsNoTracking();

            if (query.StartDate.HasValue)
                dbQuery = dbQuery.Where(m => m.OccurredAt >= query.StartDate.Value);

            if (query.EndDate.HasValue)
                dbQuery = dbQuery.Where(m => m.OccurredAt <= query.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(query.MovementTypeCode))
                dbQuery = dbQuery.Where(m => m.MovementTypeCode == query.MovementTypeCode);

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(m => m.FromWarehouseId == query.WarehouseId.Value || m.ToWarehouseId == query.WarehouseId.Value);

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(m => m.Product.Name.Contains(query.Search) || m.ReferenceNumber.Contains(query.Search));

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderByDescending(m => m.OccurredAt)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new Response(
                    m.Id, m.ProductId, m.Product.Name, m.VariantId,
                    m.MovementTypeCode, m.MovementTypeCodeNavigation.DisplayName,
                    m.FromWarehouseId, m.FromWarehouse.Name,
                    m.ToWarehouseId, m.ToWarehouse.Name,
                    m.QuantityIn, m.QuantityOut, m.BalanceAfter,
                    m.ReferenceType, m.ReferenceNumber, m.OccurredAt))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
