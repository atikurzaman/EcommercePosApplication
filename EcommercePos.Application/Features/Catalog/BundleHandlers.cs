using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

// ── GetBundleComponents ─────────────────────────────────────────────────────

public static class GetBundleComponents
{
    public sealed record Query(Guid BundleProductId);

    public sealed record Response(
        Guid Id, Guid ComponentVariantId, string VariantName, string ProductName,
        decimal Quantity, bool IsSubstitutable, int SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.BundleComponents
                .AsNoTracking()
                .Where(c => c.BundleProductId == query.BundleProductId && !c.IsDeleted)
                .Join(_context.ProductVariants.Where(v => !v.IsDeleted),
                    c => c.ComponentVariantId, v => v.Id,
                    (c, v) => new { Component = c, Variant = v })
                .Join(_context.Products.Where(p => !p.IsDeleted),
                    cv => cv.Variant.ProductId, p => p.Id,
                    (cv, p) => new Response(
                        cv.Component.Id,
                        cv.Component.ComponentVariantId,
                        cv.Variant.Name,
                        p.Name,
                        cv.Component.Quantity,
                        cv.Component.IsSubstitutable,
                        cv.Component.SortOrder))
                .OrderBy(r => r.SortOrder)
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}

// ── ManageBundleComponents ──────────────────────────────────────────────────

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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

// ── GetBundleOptionGroups ───────────────────────────────────────────────────

public static class GetBundleOptionGroups
{
    public sealed record Query(Guid BundleProductId);

    public sealed record BundleOptionItemInfo(
        Guid Id, Guid VariantId, string VariantName, string ProductName,
        decimal PriceAdjustment, bool IsDefault, int SortOrder);

    public sealed record Response(
        Guid Id, string GroupName, bool IsRequired,
        int MinSelections, int MaxSelections, int QuantityPerSelection, int SortOrder,
        List<BundleOptionItemInfo> Items);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var groups = await _context.BundleOptionGroups
                .AsNoTracking()
                .Where(g => g.BundleProductId == query.BundleProductId && !g.IsDeleted)
                .OrderBy(g => g.SortOrder)
                .Select(g => new Response(
                    g.Id,
                    g.GroupName,
                    g.IsRequired,
                    g.MinSelections,
                    g.MaxSelections,
                    g.QuantityPerSelection,
                    g.SortOrder,
                    g.BundleOptionItems
                        .Where(i => !i.IsDeleted)
                        .OrderBy(i => i.SortOrder)
                        .Join(_context.ProductVariants.Where(v => !v.IsDeleted),
                            i => i.VariantId, v => v.Id,
                            (i, v) => new { Item = i, Variant = v })
                        .Join(_context.Products.Where(p => !p.IsDeleted),
                            iv => iv.Variant.ProductId, p => p.Id,
                            (iv, p) => new BundleOptionItemInfo(
                                iv.Item.Id,
                                iv.Item.VariantId,
                                iv.Variant.Name,
                                p.Name,
                                iv.Item.PriceAdjustment,
                                iv.Item.IsDefault,
                                iv.Item.SortOrder))
                        .ToList()))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(groups);
        }
    }
}

// ── CreateBundleOptionGroup ─────────────────────────────────────────────────

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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

// ── UpdateBundleOptionGroup ─────────────────────────────────────────────────

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

// ── DeleteBundleOptionGroup ─────────────────────────────────────────────────

public static class DeleteBundleOptionGroup
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var group = await _context.BundleOptionGroups
                .Where(g => g.Id == command.Id && !g.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (group == null)
                return Result.Failure(Error.NotFound($"Bundle option group with id '{command.Id}' was not found."));

            group.IsDeleted = true;
            group.UpdatedAt = DateTime.UtcNow;

            // Soft delete associated items
            var items = await _context.BundleOptionItems
                .Where(i => i.GroupId == command.Id && !i.IsDeleted)
                .ToListAsync(ct);

            foreach (var item in items)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
