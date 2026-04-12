using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

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
