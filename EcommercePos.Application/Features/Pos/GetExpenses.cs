using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetExpenses ────────────────────────────────────────────────────────────────
public static class GetExpenses
{
    public sealed record Request(
        int PageIndex = 0, int PageSize = 10,
        Guid? WarehouseId = null, Guid? ExpenseCategoryId = null,
        DateTime? DateFrom = null, DateTime? DateTo = null,
        string? Search = null);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        Guid? ExpenseCategoryId, string? CategoryName,
        DateTime ExpenseDate, string? Description, decimal Amount,
        string? MethodCode, string? ReceiptReference);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Expenses
                .AsNoTracking()
                .Where(e => !e.IsDeleted);

            if (request.WarehouseId.HasValue)
                query = query.Where(e => e.WarehouseId == request.WarehouseId.Value);
            if (request.ExpenseCategoryId.HasValue)
                query = query.Where(e => e.ExpenseCategoryId == request.ExpenseCategoryId.Value);
            if (request.DateFrom.HasValue)
                query = query.Where(e => e.ExpenseDate >= request.DateFrom.Value);
            if (request.DateTo.HasValue)
                query = query.Where(e => e.ExpenseDate <= request.DateTo.Value);
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(e => e.Description != null && e.Description.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(e => e.ExpenseDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new Response(
                    e.Id, e.WarehouseId, e.Warehouse.Name,
                    e.ExpenseCategoryId,
                    e.ExpenseCategory != null ? e.ExpenseCategory.Name : null,
                    e.ExpenseDate, e.Description, e.Amount,
                    e.MethodCode, e.ReceiptReference))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
