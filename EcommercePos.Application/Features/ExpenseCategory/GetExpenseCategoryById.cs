using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ExpenseCategory;

public static class GetExpenseCategoryById
{
    public sealed record Query(Guid Id);

    public sealed record Response(Guid Id, string Name, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var category = await _context.ExpenseCategories
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (category is null)
                return Result<Response>.Failure(Error.NotFound($"Expense category '{query.Id}' was not found."));

            return Result<Response>.Success(
                new Response(category.Id, category.Name, category.Description, category.IsActive));
        }
    }
}
