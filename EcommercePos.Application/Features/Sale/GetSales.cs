using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Sale;

public static class GetSales
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, string? Search = null, string? Status = null);

    public sealed record Response(
        Guid Id, string OrderNumber, DateTime OrderDate, decimal TotalAmount, decimal PaidAmount,
        string StatusCode, string PaymentStatus);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Orders
                .Where(o => !o.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(o => o.OrderNumber.Contains(query.Search));

            if (!string.IsNullOrWhiteSpace(query.Status))
                dbQuery = dbQuery.Where(o => o.StatusCode == query.Status);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderByDescending(o => o.OrderDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(o => new Response(
                    o.Id, o.OrderNumber, o.OrderDate, o.TotalAmount, o.PaidAmount,
                    o.StatusCode, o.TotalAmount - o.PaidAmount > 0 ? "PARTIAL" : "PAID"))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
