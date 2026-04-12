using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
