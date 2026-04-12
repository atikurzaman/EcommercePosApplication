using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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

// ── DeleteExpense ──────────────────────────────────────────────────────────────
public static class DeleteExpense
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == command.Id && !e.IsDeleted, ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("Expense not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
