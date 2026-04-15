using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
