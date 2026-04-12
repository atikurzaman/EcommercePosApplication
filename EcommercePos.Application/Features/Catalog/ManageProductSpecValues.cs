using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class ManageProductSpecValues
{
    public sealed record SpecValueInput(Guid SpecId, string Value);

    public sealed record Request
    {
        public Guid ProductId { get; init; }
        public List<SpecValueInput> Values { get; init; } = new();
    }

    public sealed record Command(Guid ProductId, List<SpecValueInput> Values);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleForEach(x => x.Values).ChildRules(v =>
            {
                v.RuleFor(x => x.SpecId).NotEmpty();
                v.RuleFor(x => x.Value).NotEmpty().MaximumLength(500);
            });
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            // Remove existing spec values for this product
            var existing = await _context.ProductSpecificationValues
                .Where(x => x.ProductId == command.ProductId && !x.IsDeleted)
                .ToListAsync(ct);

            foreach (var e in existing)
            {
                e.IsDeleted = true;
                e.UpdatedAt = DateTime.UtcNow;
            }

            // Add new values
            foreach (var input in command.Values)
            {
                _context.ProductSpecificationValues.Add(new ProductSpecificationValues
                {
                    Id = Guid.NewGuid(),
                    ProductId = command.ProductId,
                    SpecId = input.SpecId,
                    Value = input.Value,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
