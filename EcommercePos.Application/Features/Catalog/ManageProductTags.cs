using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
