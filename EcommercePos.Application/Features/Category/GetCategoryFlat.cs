using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category;

public static class GetCategoryFlat
{
    public sealed record Query();
    public sealed record Response(Guid Id, string Name, Guid? ParentCategoryId, int DisplayOrder);
    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;
        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name)
                .Select(c => new Response(c.Id, c.Name, c.ParentCategoryId, c.DisplayOrder))
                .ToListAsync(ct);
            return Result<List<Response>>.Success(categories);
        }
    }
}