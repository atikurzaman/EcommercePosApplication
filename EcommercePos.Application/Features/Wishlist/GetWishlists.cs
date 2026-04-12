using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Wishlist;

public static class GetWishlists
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public int ItemCount { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(Guid CustomerId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var wishlists = await _context.Wishlists
                .Where(w => w.CustomerId == query.CustomerId && !w.IsDeleted)
                .Include(w => w.WishlistItems.Where(i => !i.IsDeleted))
                .AsNoTracking()
                .ToListAsync(ct);

            var result = wishlists.Select(w => new Response
            {
                Id = w.Id,
                CustomerId = w.CustomerId ?? Guid.Empty,
                ItemCount = w.WishlistItems.Count,
                CreatedAt = w.CreatedAt
            }).ToList();

            return Result<List<Response>>.Success(result);
        }
    }
}
