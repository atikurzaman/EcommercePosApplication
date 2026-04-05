using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category.Queries;

public static class GetCategoryById
{
    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<GetCategories.Response>> Handle(Query query, CancellationToken ct)
        {
            var category = await _context.Categories
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (category == null)
                return Result<GetCategories.Response>.Failure(Error.NotFound("Category not found"));

            return Result<GetCategories.Response>.Success(category.Adapt<GetCategories.Response>());
        }
    }
}
