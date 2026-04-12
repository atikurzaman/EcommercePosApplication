using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── CreateExpense ──────────────────────────────────────────────────────────────
public static class CreateExpense
{
    public sealed record Request(
        Guid WarehouseId, Guid? ExpenseCategoryId,
        DateTime ExpenseDate, string? Description, decimal Amount,
        string? MethodCode, string? ReceiptReference, Guid? CreatedByUserId);

    public sealed record Response(Guid Id, decimal Amount, DateTime ExpenseDate);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.ReceiptReference).MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var entity = new Expenses
            {
                Id = Guid.NewGuid(),
                WarehouseId = request.WarehouseId,
                ExpenseCategoryId = request.ExpenseCategoryId,
                ExpenseDate = request.ExpenseDate,
                Description = request.Description,
                Amount = request.Amount,
                MethodCode = request.MethodCode,
                ReceiptReference = request.ReceiptReference,
                CreatedByUserId = request.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Expenses.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Amount, entity.ExpenseDate));
        }
    }
}
