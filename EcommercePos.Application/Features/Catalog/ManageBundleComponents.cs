using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class ManageBundleComponents
{
    public sealed record ComponentInput(Guid ComponentVariantId, decimal Quantity, bool IsSubstitutable, int SortOrder);
    public sealed record Command(Guid BundleProductId, List<ComponentInput> Components);
    public sealed record Response(int Count);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.BundleProductId).NotEmpty();
            RuleFor(x => x.Components).NotNull();
            RuleForEach(x => x.Components).ChildRules(c =>
            {
                c.RuleFor(x => x.ComponentVariantId).NotEmpty();
                c.RuleFor(x => x.Quantity).GreaterThan(0);
            });
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            // Soft delete existing components
            var existing = await _context.BundleComponents
                .Where(c => c.BundleProductId == command.BundleProductId && !c.IsDeleted)
                .ToListAsync(ct);

            foreach (var item in existing)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
            }

            // Add new components
            foreach (var input in command.Components)
            {
                _context.BundleComponents.Add(new BundleComponents
                {
                    Id = Guid.NewGuid(),
                    BundleProductId = command.BundleProductId,
                    ComponentVariantId = input.ComponentVariantId,
                    Quantity = input.Quantity,
                    IsSubstitutable = input.IsSubstitutable,
                    SortOrder = input.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(command.Components.Count));
        }
    }
}
