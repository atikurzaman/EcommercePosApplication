using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Purchase;

public static class GetPurchases
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, string? Search = null, string? Status = null);

    public sealed record Response(
        Guid Id, string OrderNumber, DateTime OrderDate, decimal GrandTotal, string Status,
        Guid SupplierId, Guid? WarehouseId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.PurchaseOrders
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(p => p.OrderNumber.Contains(query.Search));

            if (!string.IsNullOrWhiteSpace(query.Status))
                dbQuery = dbQuery.Where(p => p.Status == query.Status);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderByDescending(p => p.OrderDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new Response(
                    p.Id, p.OrderNumber, p.OrderDate, p.GrandTotal, p.Status,
                    p.SupplierId, p.WarehouseId))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
