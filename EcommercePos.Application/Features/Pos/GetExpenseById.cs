using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetExpenseById ─────────────────────────────────────────────────────────────
public static class GetExpenseById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        Guid? ExpenseCategoryId, string? CategoryName,
        DateTime ExpenseDate, string? Description, decimal Amount,
        string? MethodCode, string? ReceiptReference,
        Guid? CreatedByUserId);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Expenses
                .AsNoTracking()
                .Where(e => e.Id == query.Id && !e.IsDeleted)
                .Select(e => new Response(
                    e.Id, e.WarehouseId, e.Warehouse.Name,
                    e.ExpenseCategoryId,
                    e.ExpenseCategory != null ? e.ExpenseCategory.Name : null,
                    e.ExpenseDate, e.Description, e.Amount,
                    e.MethodCode, e.ReceiptReference,
                    e.CreatedByUserId))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Expense not found."));

            return Result<Response>.Success(entity);
        }
    }
}
