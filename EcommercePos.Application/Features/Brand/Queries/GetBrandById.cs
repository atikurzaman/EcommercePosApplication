using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Brand.Queries;

public static class GetBrandById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public string BrandCode { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? LogoUrl { get; init; }
        public string? Website { get; init; }
        public string? CountryOfOrigin { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Brands
                .Where(b => b.Id == query.Id && !b.IsDeleted)
                .AsNoTracking()
                .ProjectToType<Response>()
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"Brand with id '{query.Id}' was not found."));
            }

            return Result<Response>.Success(item);
        }
    }
}
