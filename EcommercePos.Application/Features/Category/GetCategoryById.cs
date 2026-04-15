using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category;

public static class GetCategoryById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string Name, string? Slug, string? Description, string? ImageUrl,
        Guid? ParentCategoryId, int DisplayOrder, bool IsFeatured, bool IsActive,
        string? MetaTitle, string? MetaDescription);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.Categories
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .AsNoTracking()
                .Select(c => new Response(c.Id, c.Name, c.Slug, c.Description, c.ImageUrl,
                    c.ParentCategoryId, c.DisplayOrder, c.IsFeatured, c.IsActive,
                    c.MetaTitle, c.MetaDescription))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Category '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
