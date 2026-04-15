using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosTransactions ─────────────────────────────────────────────────────────
public static class GetPosTransactions
{
    public sealed record Request(
        int PageIndex = 0,
        int PageSize = 10,
        string? Search = null,
        Guid? CashShiftId = null,
        Guid? CashierId = null,
        Guid? WarehouseId = null,
        string? Status = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null);

    public sealed record Response(
        Guid Id,
        string ReceiptNumber,
        DateTime SaleDate,
        string Status,
        string? CustomerName,
        string? CustomerPhone,
        decimal SubTotal,
        decimal DiscountAmount,
        decimal TotalTaxAmount,
        decimal GrandTotal,
        decimal PaidAmount,
        decimal ChangeAmount,
        int ItemCount,
        string CashierName,
        string WarehouseName);

    public sealed record Query(
        int PageIndex,
        int PageSize,
        string? Search,
        Guid? CashShiftId,
        Guid? CashierId,
        Guid? WarehouseId,
        string? Status,
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
            var dbQuery = _context.PosTransactions
                .Where(t => !t.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(t =>
                    t.ReceiptNumber.Contains(query.Search) ||
                    (t.CustomerName != null && t.CustomerName.Contains(query.Search)) ||
                    (t.CustomerPhone != null && t.CustomerPhone.Contains(query.Search)));
            }

            if (query.CashShiftId.HasValue)
                dbQuery = dbQuery.Where(t => t.CashShiftId == query.CashShiftId.Value);

            if (query.CashierId.HasValue)
                dbQuery = dbQuery.Where(t => t.CashierId == query.CashierId.Value);

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(t => t.WarehouseId == query.WarehouseId.Value);

            if (!string.IsNullOrWhiteSpace(query.Status))
                dbQuery = dbQuery.Where(t => t.Status == query.Status);

            if (query.DateFrom.HasValue)
                dbQuery = dbQuery.Where(t => t.SaleDate >= query.DateFrom.Value);

            if (query.DateTo.HasValue)
                dbQuery = dbQuery.Where(t => t.SaleDate <= query.DateTo.Value);

            var totalCount = await dbQuery.CountAsync(ct);

            var items = await dbQuery
                .Include(t => t.Cashier)
                .Include(t => t.Warehouse)
                .Include(t => t.PosTransactionLines)
                .OrderByDescending(t => t.SaleDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new Response(
                    t.Id,
                    t.ReceiptNumber,
                    t.SaleDate,
                    t.Status,
                    t.CustomerName,
                    t.CustomerPhone,
                    t.SubTotal,
                    t.DiscountAmount,
                    t.TotalTaxAmount,
                    t.GrandTotal,
                    t.PaidAmount,
                    t.ChangeAmount,
                    t.PosTransactionLines.Count(l => !l.IsDeleted),
                    t.Cashier.UserName ?? string.Empty,
                    t.Warehouse.Name))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
