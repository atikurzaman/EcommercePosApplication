using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ReorderRule;

public static class GetReorderRules
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, Guid? WarehouseId = null, bool? ActiveOnly = null);

    public sealed record Response(
        Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
        Guid? WarehouseId, string? WarehouseName,
        Guid? PreferredSupplierId, string? PreferredSupplierName,
        decimal ReorderLevel, decimal ReorderQuantity, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.ReorderRules
                .Include(r => r.Product)
                .Include(r => r.Warehouse)
                .Include(r => r.PreferredSupplier)
                .Where(r => !r.IsDeleted)
                .AsNoTracking();

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(r => r.WarehouseId == query.WarehouseId.Value);

            if (query.ActiveOnly == true)
                dbQuery = dbQuery.Where(r => r.IsActive);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(r => r.Product.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new Response(
                    r.Id, r.ProductId, r.Product.Name, r.VariantId,
                    r.WarehouseId, r.Warehouse != null ? r.Warehouse.Name : null,
                    r.PreferredSupplierId, r.PreferredSupplier != null ? r.PreferredSupplier.Name : null,
                    r.ReorderLevel, r.ReorderQuantity, r.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
