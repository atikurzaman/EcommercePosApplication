using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

// ── GetCollections ──────────────────────────────────────────────────────────

public static class GetCollections
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, string Name, string Slug, string? Description, string? ImageUrl,
        int DisplayOrder, bool IsActive, bool ShowInHomePage, int ProductCount);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.ProductCollections
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.Name.Contains(request.Search) || c.Slug.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.DisplayOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(
                    c.Id, c.Name, c.Slug, c.Description, c.ImageUrl,
                    c.DisplayOrder, c.IsActive, c.ShowInHomePage,
                    c.ProductCollectionItems.Count(i => !i.IsDeleted)))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

// ── GetCollectionById ───────────────────────────────────────────────────────

public static class GetCollectionById
{
    public sealed record Query(Guid Id);

    public sealed record CollectionProductInfo(
        Guid Id, Guid ProductId, string ProductName, string? ProductCode,
        string? ImageUrl, decimal SalePrice, int DisplayOrder);

    public sealed record Response(
        Guid Id, string Name, string Slug, string? Description, string? ImageUrl,
        int DisplayOrder, bool IsActive, bool ShowInHomePage,
        List<CollectionProductInfo> Products);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.ProductCollections
                .AsNoTracking()
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .Select(c => new Response(
                    c.Id, c.Name, c.Slug, c.Description, c.ImageUrl,
                    c.DisplayOrder, c.IsActive, c.ShowInHomePage,
                    c.ProductCollectionItems
                        .Where(i => !i.IsDeleted)
                        .Join(_context.Products.Where(p => !p.IsDeleted),
                            i => i.ProductId, p => p.Id,
                            (i, p) => new CollectionProductInfo(
                                i.Id, i.ProductId, p.Name, p.ProductCode,
                                p.ProductImages
                                    .Where(img => !img.IsDeleted)
                                    .OrderBy(img => img.SortOrder)
                                    .Select(img => img.ImageUrl)
                                    .FirstOrDefault(),
                                p.SalePrice,
                                i.DisplayOrder))
                        .OrderBy(x => x.DisplayOrder)
                        .ToList()))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound($"Collection with id '{query.Id}' was not found."));

            return Result<Response>.Success(entity);
        }
    }
}

// ── CreateCollection ────────────────────────────────────────────────────────

public static class CreateCollection
{
    public sealed record Request(
        string Name, string? Slug, string? Description, string? ImageUrl,
        int DisplayOrder, bool IsActive, bool ShowInHomePage);

    public sealed record Response(Guid Id, string Name, string Slug);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.ImageUrl).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-").Replace("--", "-");

            var entity = new ProductCollections
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive,
                ShowInHomePage = request.ShowInHomePage,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ProductCollections.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Name, entity.Slug));
        }
    }
}

// ── UpdateCollection ────────────────────────────────────────────────────────

public static class UpdateCollection
{
    public sealed record Command(
        Guid Id, string Name, string? Slug, string? Description, string? ImageUrl,
        int DisplayOrder, bool IsActive, bool ShowInHomePage);

    public sealed record Response(Guid Id, string Name, string Slug);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Slug).MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.ImageUrl).MaximumLength(500);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ProductCollections
                .Where(c => c.Id == command.Id && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound($"Collection with id '{command.Id}' was not found."));

            entity.Name = command.Name;
            entity.Slug = command.Slug ?? command.Name.ToLower().Replace(" ", "-").Replace("--", "-");
            entity.Description = command.Description;
            entity.ImageUrl = command.ImageUrl;
            entity.DisplayOrder = command.DisplayOrder;
            entity.IsActive = command.IsActive;
            entity.ShowInHomePage = command.ShowInHomePage;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(entity.Id, entity.Name, entity.Slug));
        }
    }
}

// ── DeleteCollection ────────────────────────────────────────────────────────

public static class DeleteCollection
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ProductCollections
                .Where(c => c.Id == command.Id && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result.Failure(Error.NotFound($"Collection with id '{command.Id}' was not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}

// ── ManageCollectionItems ───────────────────────────────────────────────────

public static class ManageCollectionItems
{
    public sealed record CollectionItemInput(Guid ProductId, int DisplayOrder);
    public sealed record Command(Guid CollectionId, List<CollectionItemInput> Items);
    public sealed record Response(int Count);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CollectionId).NotEmpty();
            RuleFor(x => x.Items).NotNull();
            RuleForEach(x => x.Items).ChildRules(c =>
            {
                c.RuleFor(x => x.ProductId).NotEmpty();
            });
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var collection = await _context.ProductCollections
                .Where(c => c.Id == command.CollectionId && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (collection == null)
                return Result<Response>.Failure(Error.NotFound($"Collection with id '{command.CollectionId}' was not found."));

            // Soft delete existing items
            var existing = await _context.ProductCollectionItems
                .Where(i => i.ProductCollectionId == command.CollectionId && !i.IsDeleted)
                .ToListAsync(ct);

            foreach (var item in existing)
                item.IsDeleted = true;

            // Add new items
            foreach (var input in command.Items)
            {
                _context.ProductCollectionItems.Add(new ProductCollectionItems
                {
                    Id = Guid.NewGuid(),
                    ProductCollectionId = command.CollectionId,
                    ProductId = input.ProductId,
                    DisplayOrder = input.DisplayOrder,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(command.Items.Count));
        }
    }
}

// ── GetHomePageCollections ──────────────────────────────────────────────────

public static class GetHomePageCollections
{
    public sealed record CollectionProductInfo(
        Guid ProductId, string ProductName, string? ProductCode,
        string? ImageUrl, decimal SalePrice, int DisplayOrder);

    public sealed record Response(
        Guid Id, string Name, string Slug, string? Description, string? ImageUrl,
        int DisplayOrder, List<CollectionProductInfo> Products);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(CancellationToken ct)
        {
            var collections = await _context.ProductCollections
                .AsNoTracking()
                .Where(c => !c.IsDeleted && c.IsActive && c.ShowInHomePage)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new Response(
                    c.Id, c.Name, c.Slug, c.Description, c.ImageUrl,
                    c.DisplayOrder,
                    c.ProductCollectionItems
                        .Where(i => !i.IsDeleted)
                        .Join(_context.Products.Where(p => !p.IsDeleted && p.IsActive),
                            i => i.ProductId, p => p.Id,
                            (i, p) => new CollectionProductInfo(
                                i.ProductId, p.Name, p.ProductCode,
                                p.ProductImages
                                    .Where(img => !img.IsDeleted)
                                    .OrderBy(img => img.SortOrder)
                                    .Select(img => img.ImageUrl)
                                    .FirstOrDefault(),
                                p.SalePrice,
                                i.DisplayOrder))
                        .OrderBy(x => x.DisplayOrder)
                        .ToList()))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(collections);
        }
    }
}
