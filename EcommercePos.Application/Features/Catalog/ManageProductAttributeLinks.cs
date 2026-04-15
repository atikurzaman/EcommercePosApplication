using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
