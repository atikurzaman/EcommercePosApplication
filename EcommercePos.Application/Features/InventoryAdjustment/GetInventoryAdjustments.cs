using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.InventoryAdjustment;

public static class GetInventoryAdjustments
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, Guid? WarehouseId = null);

    public sealed record Response(
        Guid Id, string AdjustmentNo, Guid WarehouseId, string WarehouseName,
        DateTime AdjustmentDate, string AdjustmentType, string Reason,
        bool IsApproved, DateTime? ApprovedAt, DateTime CreatedAt, string? CreatedBy);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.InventoryAdjustments
                .Include(a => a.Warehouse)
                .Include(a => a.CreatedByNavigation)
                .Where(a => !a.IsDeleted)
                .AsNoTracking();

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(a => a.WarehouseId == query.WarehouseId.Value);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderByDescending(a => a.AdjustmentDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(a => new Response(
                    a.Id, a.AdjustmentNo, a.WarehouseId, a.Warehouse.Name,
                    a.AdjustmentDate, a.AdjustmentType, a.Reason,
                    a.ApprovedByUserId != null, a.ApprovedAt, a.CreatedAt,
                    a.CreatedByNavigation != null ? a.CreatedByNavigation.FirstName + " " + a.CreatedByNavigation.LastName : null))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
