using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.StockTransfer;

public static class GetStockTransfers
{
    public sealed record Query(
        int PageIndex = 0, int PageSize = 10,
        Guid? FromWarehouseId = null, Guid? ToWarehouseId = null, string? Status = null);

    public sealed record Response(
        Guid Id, string TransferNo, Guid FromWarehouseId, string FromWarehouseName,
        Guid ToWarehouseId, string ToWarehouseName, DateTime TransferDate, string Status,
        DateTime CreatedAt, string? CreatedBy);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.StockTransfers
                .Include(t => t.FromWarehouse)
                .Include(t => t.ToWarehouse)
                .Include(t => t.CreatedByNavigation)
                .Where(t => !t.IsDeleted)
                .AsNoTracking();

            if (query.FromWarehouseId.HasValue)
                dbQuery = dbQuery.Where(t => t.FromWarehouseId == query.FromWarehouseId.Value);

            if (query.ToWarehouseId.HasValue)
                dbQuery = dbQuery.Where(t => t.ToWarehouseId == query.ToWarehouseId.Value);

            if (!string.IsNullOrWhiteSpace(query.Status))
                dbQuery = dbQuery.Where(t => t.Status == query.Status);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderByDescending(t => t.TransferDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new Response(
                    t.Id, t.TransferNo, t.FromWarehouseId, t.FromWarehouse.Name,
                    t.ToWarehouseId, t.ToWarehouse.Name, t.TransferDate, t.Status,
                    t.CreatedAt,
                    t.CreatedByNavigation != null ? t.CreatedByNavigation.FirstName + " " + t.CreatedByNavigation.LastName : null))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
