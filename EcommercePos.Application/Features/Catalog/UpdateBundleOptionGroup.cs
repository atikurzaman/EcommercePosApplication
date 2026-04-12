using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class UpdateBundleOptionGroup
{
    public sealed record OptionItemInput(Guid VariantId, decimal PriceAdjustment, bool IsDefault, int SortOrder);

    public sealed record Command(
        Guid Id, string GroupName, bool IsRequired,
        int MinSelections, int MaxSelections, int QuantityPerSelection, int SortOrder,
        List<OptionItemInput>? Items);

    public sealed record Response(Guid Id, string GroupName);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.GroupName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.MinSelections).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxSelections).GreaterThanOrEqualTo(0);
            RuleFor(x => x.QuantityPerSelection).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var group = await _context.BundleOptionGroups
                .Where(g => g.Id == command.Id && !g.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (group == null)
                return Result<Response>.Failure(Error.NotFound($"Bundle option group with id '{command.Id}' was not found."));

            group.GroupName = command.GroupName;
            group.IsRequired = command.IsRequired;
            group.MinSelections = command.MinSelections;
            group.MaxSelections = command.MaxSelections;
            group.QuantityPerSelection = command.QuantityPerSelection;
            group.SortOrder = command.SortOrder;
            group.UpdatedAt = DateTime.UtcNow;

            // Replace items if provided
            if (command.Items is not null)
            {
                var existingItems = await _context.BundleOptionItems
                    .Where(i => i.GroupId == command.Id && !i.IsDeleted)
                    .ToListAsync(ct);

                foreach (var item in existingItems)
                {
                    item.IsDeleted = true;
                    item.UpdatedAt = DateTime.UtcNow;
                }

                foreach (var input in command.Items)
                {
                    _context.BundleOptionItems.Add(new BundleOptionItems
                    {
                        Id = Guid.NewGuid(),
                        GroupId = command.Id,
                        VariantId = input.VariantId,
                        PriceAdjustment = input.PriceAdjustment,
                        IsDefault = input.IsDefault,
                        SortOrder = input.SortOrder,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(group.Id, group.GroupName));
        }
    }
}
