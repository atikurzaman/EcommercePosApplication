using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Brand;

public static class GetBrandsWithCount
{
    public sealed record Query();
    public sealed record Response(
        Guid Id, string? BrandCode, string Name, string? Slug, string? Description,
        string? LogoUrl, string? Website, bool IsFeatured, bool IsActive, int ProductCount);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var brands = await _context.Brands
                .Where(b => !b.IsDeleted)
                .OrderBy(b => b.Name)
                .Select(b => new Response(
                    b.Id, b.BrandCode, b.Name, b.Slug, b.Description,
                    b.LogoUrl, b.Website, b.IsFeatured, b.IsActive,
                    _context.Products.Count(p => p.BrandId == b.Id && !p.IsDeleted)))
                .ToListAsync(ct);
            return Result<List<Response>>.Success(brands);
        }
    }
}