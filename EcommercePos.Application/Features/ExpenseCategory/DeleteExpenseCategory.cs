using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ExpenseCategory;

public static class DeleteExpenseCategory
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var category = await _context.ExpenseCategories
                .Where(c => c.Id == command.Id && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (category is null)
                return Result.Failure(Error.NotFound($"Expense category '{command.Id}' was not found."));

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
