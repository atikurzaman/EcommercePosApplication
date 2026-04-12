using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetProductAttributeLinks
{
    public sealed record Request(Guid ProductId);

    public sealed record AttributeOptionInfo(
        Guid Id, string Value, string? DisplayValue, Guid? ColorId, int SortOrder, bool IsActive);

    public sealed record Response(
        Guid Id, Guid AttributeTypeId, string AttributeTypeName, string UiType,
        bool IsRequired, int SortOrder, List<AttributeOptionInfo> Options);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Request request, CancellationToken ct)
        {
            var items = await _context.ProductAttributeLinks
                .AsNoTracking()
                .Where(l => l.ProductId == request.ProductId && !l.IsDeleted)
                .OrderBy(l => l.SortOrder)
                .Select(l => new Response(
                    l.Id, l.AttributeTypeId, l.AttributeType.Name, l.AttributeType.UiType,
                    l.IsRequired, l.SortOrder,
                    l.AttributeType.AttributeOptions
                        .Where(o => !o.IsDeleted)
                        .OrderBy(o => o.SortOrder)
                        .Select(o => new AttributeOptionInfo(
                            o.Id, o.Value, o.DisplayValue, o.ColorId, o.SortOrder, o.IsActive))
                        .ToList()))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}

public static class ManageProductAttributeLinks
{
    public sealed record AttributeLinkInput(Guid AttributeTypeId, bool IsRequired, int SortOrder);

    public sealed record Command(Guid ProductId, List<AttributeLinkInput> Links);

    public sealed record Response(int LinkedCount);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Links).NotNull();
            RuleForEach(x => x.Links).ChildRules(link =>
            {
                link.RuleFor(l => l.AttributeTypeId).NotEmpty();
            });
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var productExists = await _context.Products
                .AnyAsync(p => p.Id == command.ProductId && !p.IsDeleted, ct);

            if (!productExists)
                return Result<Response>.Failure(Error.NotFound("Product not found."));

            // Soft-delete existing links
            var existingLinks = await _context.ProductAttributeLinks
                .Where(l => l.ProductId == command.ProductId && !l.IsDeleted)
                .ToListAsync(ct);

            foreach (var link in existingLinks)
                link.IsDeleted = true;

            // Add new links
            foreach (var input in command.Links)
            {
                _context.ProductAttributeLinks.Add(new ProductAttributeLinks
                {
                    Id = Guid.NewGuid(),
                    ProductId = command.ProductId,
                    AttributeTypeId = input.AttributeTypeId,
                    IsRequired = input.IsRequired,
                    SortOrder = input.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(command.Links.Count));
        }
    }
}
