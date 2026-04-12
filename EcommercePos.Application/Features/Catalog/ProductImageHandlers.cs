using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

// ── GetProductImages ────────────────────────────────────────────────────────
public static class GetProductImages
{
    public sealed record Request(Guid ProductId, Guid? VariantId = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public Guid? VariantId { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public string? AltText { get; init; }
        public int SortOrder { get; init; }
        public bool IsPrimary { get; init; }
    }

    public sealed record Query(Guid ProductId, Guid? VariantId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.ProductImages
                .Where(x => x.ProductId == query.ProductId && !x.IsDeleted);

            if (query.VariantId.HasValue)
                q = q.Where(x => x.VariantId == query.VariantId.Value);

            var items = await q
                .OrderBy(x => x.SortOrder)
                .Select(x => new Response
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    ImageUrl = x.ImageUrl,
                    AltText = x.AltText,
                    SortOrder = x.SortOrder,
                    IsPrimary = x.IsPrimary
                })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}

// ── AddProductImage ─────────────────────────────────────────────────────────
public static class AddProductImage
{
    public sealed record Request
    {
        public Guid ProductId { get; init; }
        public Guid? VariantId { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public string? AltText { get; init; }
        public int SortOrder { get; init; }
        public bool IsPrimary { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public Guid? VariantId { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public string? AltText { get; init; }
        public int SortOrder { get; init; }
        public bool IsPrimary { get; init; }
    }

    public sealed record Command(
        Guid ProductId, Guid? VariantId, string ImageUrl, string? AltText,
        int SortOrder, bool IsPrimary);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.AltText).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            if (command.IsPrimary)
            {
                var existingPrimaries = await _context.ProductImages
                    .Where(x => x.ProductId == command.ProductId
                                && x.VariantId == command.VariantId
                                && x.IsPrimary && !x.IsDeleted)
                    .ToListAsync(ct);

                foreach (var p in existingPrimaries)
                    p.IsPrimary = false;
            }

            var item = new ProductImages
            {
                Id = Guid.NewGuid(),
                ProductId = command.ProductId,
                VariantId = command.VariantId,
                ImageUrl = command.ImageUrl,
                AltText = command.AltText,
                SortOrder = command.SortOrder,
                IsPrimary = command.IsPrimary,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ProductImages.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                ImageUrl = item.ImageUrl,
                AltText = item.AltText,
                SortOrder = item.SortOrder,
                IsPrimary = item.IsPrimary
            });
        }
    }
}

// ── UpdateProductImage ──────────────────────────────────────────────────────
public static class UpdateProductImage
{
    public sealed record Request
    {
        public string? AltText { get; init; }
        public int SortOrder { get; init; }
        public bool IsPrimary { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public Guid? VariantId { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public string? AltText { get; init; }
        public int SortOrder { get; init; }
        public bool IsPrimary { get; init; }
    }

    public sealed record Command(Guid Id, string? AltText, int SortOrder, bool IsPrimary);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.AltText).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.ProductImages
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result<Response>.Failure(Error.NotFound($"ProductImage with id '{command.Id}' was not found."));

            if (command.IsPrimary && !item.IsPrimary)
            {
                var existingPrimaries = await _context.ProductImages
                    .Where(x => x.ProductId == item.ProductId
                                && x.VariantId == item.VariantId
                                && x.Id != item.Id
                                && x.IsPrimary && !x.IsDeleted)
                    .ToListAsync(ct);

                foreach (var p in existingPrimaries)
                    p.IsPrimary = false;
            }

            item.AltText = command.AltText;
            item.SortOrder = command.SortOrder;
            item.IsPrimary = command.IsPrimary;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                ImageUrl = item.ImageUrl,
                AltText = item.AltText,
                SortOrder = item.SortOrder,
                IsPrimary = item.IsPrimary
            });
        }
    }
}

// ── DeleteProductImage ──────────────────────────────────────────────────────
public static class DeleteProductImage
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.ProductImages
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result.Failure(Error.NotFound($"ProductImage with id '{command.Id}' was not found."));

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}

// ── ReorderProductImages ────────────────────────────────────────────────────
public static class ReorderProductImages
{
    public sealed record ImageOrder(Guid ImageId, int SortOrder);

    public sealed record Request
    {
        public Guid ProductId { get; init; }
        public List<ImageOrder> Orders { get; init; } = new();
    }

    public sealed record Command(Guid ProductId, List<ImageOrder> Orders);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Orders).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var imageIds = command.Orders.Select(o => o.ImageId).ToList();

            var images = await _context.ProductImages
                .Where(x => x.ProductId == command.ProductId
                            && imageIds.Contains(x.Id)
                            && !x.IsDeleted)
                .ToListAsync(ct);

            var orderMap = command.Orders.ToDictionary(o => o.ImageId, o => o.SortOrder);

            foreach (var img in images)
            {
                if (orderMap.TryGetValue(img.Id, out var sortOrder))
                {
                    img.SortOrder = sortOrder;
                    img.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
