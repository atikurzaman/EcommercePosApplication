using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Brand;

public static class GetBrandById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string BrandCode, string Name, string? Description, string? LogoUrl,
        string? Website, string? CountryOfOrigin, bool IsFeatured, bool IsActive, DateTime CreatedAt);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Brands
                .Where(b => b.Id == query.Id && !b.IsDeleted)
                .AsNoTracking()
                .Select(b => new Response(b.Id, b.BrandCode, b.Name, b.Description, b.LogoUrl,
                    b.Website, b.CountryOfOrigin, b.IsFeatured, b.IsActive, b.CreatedAt))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Brand '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
