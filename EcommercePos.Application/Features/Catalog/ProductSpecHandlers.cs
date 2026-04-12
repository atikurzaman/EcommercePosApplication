using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

// ── GetSpecifications ───────────────────────────────────────────────────────
public static class GetSpecifications
{
    public sealed record Request(int PageIndex = 0, int PageSize = 50, string? Search = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string SpecName { get; init; } = string.Empty;
        public int SortOrder { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, string? Search);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.ProductSpecifications
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.Search))
                q = q.Where(x => x.SpecName.Contains(query.Search));

            var totalCount = await q.CountAsync(ct);

            var items = await q
                .OrderBy(x => x.SortOrder)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new Response
                {
                    Id = x.Id,
                    SpecName = x.SpecName,
                    SortOrder = x.SortOrder
                })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}

// ── CreateSpecification ─────────────────────────────────────────────────────
public static class CreateSpecification
{
    public sealed record Request
    {
        public string SpecName { get; init; } = string.Empty;
        public int SortOrder { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string SpecName { get; init; } = string.Empty;
        public int SortOrder { get; init; }
    }

    public sealed record Command(string SpecName, int SortOrder);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.SpecName).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = new ProductSpecifications
            {
                Id = Guid.NewGuid(),
                SpecName = command.SpecName,
                SortOrder = command.SortOrder,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ProductSpecifications.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = item.Id,
                SpecName = item.SpecName,
                SortOrder = item.SortOrder
            });
        }
    }
}

// ── GetProductSpecValues ────────────────────────────────────────────────────
public static class GetProductSpecValues
{
    public sealed record Request(Guid ProductId);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid SpecId { get; init; }
        public string SpecName { get; init; } = string.Empty;
        public Guid? VariantId { get; init; }
        public string Value { get; init; } = string.Empty;
    }

    public sealed record Query(Guid ProductId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var items = await _context.ProductSpecificationValues
                .Where(x => x.ProductId == query.ProductId && !x.IsDeleted)
                .Join(
                    _context.ProductSpecifications.Where(s => !s.IsDeleted),
                    v => v.SpecId,
                    s => s.Id,
                    (v, s) => new Response
                    {
                        Id = v.Id,
                        SpecId = v.SpecId,
                        SpecName = s.SpecName,
                        VariantId = v.VariantId,
                        Value = v.Value
                    })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}

// ── ManageProductSpecValues ─────────────────────────────────────────────────
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
