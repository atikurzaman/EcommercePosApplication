using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── UpdateExpense ──────────────────────────────────────────────────────────────
public static class UpdateExpense
{
    public sealed record Request(
        Guid? ExpenseCategoryId, DateTime ExpenseDate,
        string? Description, decimal Amount,
        string? MethodCode, string? ReceiptReference);

    public sealed record Command(
        Guid Id, Guid? ExpenseCategoryId, DateTime ExpenseDate,
        string? Description, decimal Amount,
        string? MethodCode, string? ReceiptReference);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.ReceiptReference).MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetExpenseById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == command.Id && !e.IsDeleted, ct);

            if (entity == null)
                return Result<GetExpenseById.Response>.Failure(Error.NotFound("Expense not found."));

            entity.ExpenseCategoryId = command.ExpenseCategoryId;
            entity.ExpenseDate = command.ExpenseDate;
            entity.Description = command.Description;
            entity.Amount = command.Amount;
            entity.MethodCode = command.MethodCode;
            entity.ReceiptReference = command.ReceiptReference;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var warehouseName = await _context.Warehouses
                .Where(w => w.Id == entity.WarehouseId)
                .Select(w => w.Name)
                .FirstOrDefaultAsync(ct) ?? string.Empty;

            var categoryName = entity.ExpenseCategoryId.HasValue
                ? await _context.ExpenseCategories
                    .Where(c => c.Id == entity.ExpenseCategoryId.Value)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync(ct)
                : null;

            return Result<GetExpenseById.Response>.Success(
                new GetExpenseById.Response(
                    entity.Id, entity.WarehouseId, warehouseName,
                    entity.ExpenseCategoryId, categoryName,
                    entity.ExpenseDate, entity.Description, entity.Amount,
                    entity.MethodCode, entity.ReceiptReference,
                    entity.CreatedByUserId));
        }
    }
}
