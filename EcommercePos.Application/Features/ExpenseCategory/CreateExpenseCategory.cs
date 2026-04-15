using FluentValidation;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ExpenseCategory;

public static class CreateExpenseCategory
{
    public sealed record Command(string Name, string? Description, bool IsActive);

    public sealed record Response(Guid Id, string Name, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.Name).NotEmpty();
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var category = new ExpenseCategories
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                IsActive = command.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ExpenseCategories.Add(category);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(category.Id, category.Name, category.IsActive));
        }
    }
}
