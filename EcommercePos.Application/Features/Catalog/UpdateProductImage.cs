using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
