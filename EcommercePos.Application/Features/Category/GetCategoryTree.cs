using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category;

public static class GetCategoryTree
{
    public sealed record Query();
    public sealed record Response(
        Guid Id, string Name, string? Slug, Guid? ParentCategoryId,
        int DisplayOrder, bool IsActive, string? ImageUrl, List<Response>? Children);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .Select(c => new Response(c.Id, c.Name, c.Slug, c.ParentCategoryId,
                    c.DisplayOrder, c.IsActive, c.ImageUrl, null))
                .ToListAsync(ct);
            return Result<List<Response>>.Success(BuildTree(categories));
        }
        private static List<Response> BuildTree(List<Response> flat)
        {
            var roots = flat.Where(c => c.ParentCategoryId == null ||
                !flat.Any(p => p.Id == c.ParentCategoryId)).ToList();
            static List<Response> GetChildren(Guid parentId, List<Response> all) =>
                all.Where(c => c.ParentCategoryId == parentId)
                   .Select(c => c with { Children = GetChildren(c.Id, all) })
                   .ToList();
            return roots.Select(r => r with { Children = GetChildren(r.Id, flat) }).ToList();
        }
    }
}