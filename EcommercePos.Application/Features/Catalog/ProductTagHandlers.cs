using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

// ── GetTags ─────────────────────────────────────────────────────────────────
public static class GetTags
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public int ProductCount { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, string? Search);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.Tags.Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.Search))
                q = q.Where(x => x.Name.Contains(query.Search) || x.Slug.Contains(query.Search));

            var totalCount = await q.CountAsync(ct);

            var items = await q
                .OrderBy(x => x.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new Response
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    ProductCount = x.ProductTags.Count
                })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}

// ── CreateTag ───────────────────────────────────────────────────────────────
public static class CreateTag
{
    public sealed record Request
    {
        public string Name { get; init; } = string.Empty;
        public string? Slug { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
    }

    public sealed record Command(string Name, string? Slug);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var slug = command.Slug ?? command.Name.ToLower().Replace(" ", "-").Replace("--", "-");

            var item = new Tags
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Slug = slug,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Tags.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                Name = item.Name,
                Slug = item.Slug
            });
        }
    }
}

// ── UpdateTag ───────────────────────────────────────────────────────────────
public static class UpdateTag
{
    public sealed record Request
    {
        public string Name { get; init; } = string.Empty;
        public string? Slug { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
    }

    public sealed record Command(Guid Id, string Name, string? Slug);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Tags
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result<Response>.Failure(Error.NotFound($"Tag with id '{command.Id}' was not found."));

            item.Name = command.Name;
            item.Slug = command.Slug ?? command.Name.ToLower().Replace(" ", "-").Replace("--", "-");
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                Name = item.Name,
                Slug = item.Slug
            });
        }
    }
}

// ── DeleteTag ───────────────────────────────────────────────────────────────
public static class DeleteTag
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.Tags
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
                return Result.Failure(Error.NotFound($"Tag with id '{command.Id}' was not found."));

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}

// ── GetProductTags ──────────────────────────────────────────────────────────
public static class GetProductTags
{
    public sealed record Request(Guid ProductId);

    public sealed record Response
    {
        public Guid TagId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
    }

    public sealed record Query(Guid ProductId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.ProductTags
                .Where(x => x.ProductId == query.ProductId)
                .Join(
                    _context.Tags.Where(t => !t.IsDeleted),
                    pt => pt.TagId,
                    t => t.Id,
                    (pt, t) => new Response
                    {
                        TagId = t.Id,
                        Name = t.Name,
                        Slug = t.Slug
                    })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}

// ── ManageProductTags ───────────────────────────────────────────────────────
public static class ManageProductTags
{
    public sealed record Request
    {
        public Guid ProductId { get; init; }
        public List<Guid> TagIds { get; init; } = new();
    }

    public sealed record Command(Guid ProductId, List<Guid> TagIds);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            // Remove existing product-tag links (hard delete)
            var existing = await _context.ProductTags
                .Where(x => x.ProductId == command.ProductId)
                .ToListAsync(ct);

            _context.ProductTags.RemoveRange(existing);

            // Add new links
            foreach (var tagId in command.TagIds)
            {
                _context.ProductTags.Add(new ProductTags
                {
                    ProductId = command.ProductId,
                    TagId = tagId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
