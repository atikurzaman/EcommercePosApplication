using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ExpenseCategory;

public static class UpdateExpenseCategory
{
    public sealed record Command(Guid Id, string Name, string? Description, bool IsActive);

    public sealed record Response(Guid Id, string Name, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.Name).NotEmpty();
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var category = await _context.ExpenseCategories
                .Where(c => c.Id == command.Id && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (category is null)
                return Result<Response>.Failure(Error.NotFound($"Expense category '{command.Id}' was not found."));

            category.Name = command.Name;
            category.Description = command.Description;
            category.IsActive = command.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(category.Id, category.Name, category.IsActive));
        }
    }
}
