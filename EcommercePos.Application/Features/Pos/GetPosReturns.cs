using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosReturns ──────────────────────────────────────────────────────────────
public static class GetPosReturns
{
    public sealed record Request(
        int PageIndex = 0,
        int PageSize = 10,
        Guid? WarehouseId = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null);

    public sealed record Response(
        Guid Id,
        string ReturnNo,
        DateTime ReturnDate,
        Guid WarehouseId,
        string WarehouseName,
        Guid? CustomerId,
        string? CustomerName,
        decimal TotalAmount,
        string? Notes,
        int ItemCount);

    public sealed record Query(
        int PageIndex,
        int PageSize,
        Guid? WarehouseId,
        DateTime? DateFrom,
        DateTime? DateTo);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.PosTransactionReturns
                .Where(r => !r.IsDeleted)
                .AsNoTracking();

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(r => r.WarehouseId == query.WarehouseId.Value);

            if (query.DateFrom.HasValue)
                dbQuery = dbQuery.Where(r => r.ReturnDate >= query.DateFrom.Value);

            if (query.DateTo.HasValue)
                dbQuery = dbQuery.Where(r => r.ReturnDate <= query.DateTo.Value);

            var totalCount = await dbQuery.CountAsync(ct);

            var items = await dbQuery
                .Include(r => r.Warehouse)
                .Include(r => r.Customer)
                .Include(r => r.PosTransactionReturnLines)
                .OrderByDescending(r => r.ReturnDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new Response(
                    r.Id,
                    r.ReturnNo,
                    r.ReturnDate,
                    r.WarehouseId,
                    r.Warehouse.Name,
                    r.CustomerId,
                    r.Customer != null ? (r.Customer.CompanyName ?? r.Customer.CustomerCode) : null,
                    r.TotalAmount,
                    r.Notes,
                    r.PosTransactionReturnLines.Count(l => !l.IsDeleted)))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
