using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class CreateBundleOptionGroup
{
    public sealed record OptionItemInput(Guid VariantId, decimal PriceAdjustment, bool IsDefault, int SortOrder);

    public sealed record Request(
        Guid BundleProductId, string GroupName, bool IsRequired,
        int MinSelections, int MaxSelections, int QuantityPerSelection, int SortOrder,
        List<OptionItemInput>? Items);

    public sealed record Response(Guid Id, string GroupName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.BundleProductId).NotEmpty();
            RuleFor(x => x.GroupName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.MinSelections).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxSelections).GreaterThanOrEqualTo(0);
            RuleFor(x => x.QuantityPerSelection).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var group = new BundleOptionGroups
            {
                Id = Guid.NewGuid(),
                BundleProductId = request.BundleProductId,
                GroupName = request.GroupName,
                IsRequired = request.IsRequired,
                MinSelections = request.MinSelections,
                MaxSelections = request.MaxSelections,
                QuantityPerSelection = request.QuantityPerSelection,
                SortOrder = request.SortOrder,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.BundleOptionGroups.Add(group);

            if (request.Items is { Count: > 0 })
            {
                foreach (var item in request.Items)
                {
                    _context.BundleOptionItems.Add(new BundleOptionItems
                    {
                        Id = Guid.NewGuid(),
                        GroupId = group.Id,
                        VariantId = item.VariantId,
                        PriceAdjustment = item.PriceAdjustment,
                        IsDefault = item.IsDefault,
                        SortOrder = item.SortOrder,
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
