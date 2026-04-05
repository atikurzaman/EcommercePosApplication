using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Category.Queries;

public static class GetCategories
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(
        Guid Id, string Name, string? Description, Guid? ParentCategoryId, 
        bool IsActive, int DisplayOrder, string? ImageUrl);

    public sealed record Query(int PageIndex, int PageSize, string? Search);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Categories
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(c => c.Name.Contains(query.Search));
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(c => c.DisplayOrder)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .ProjectToType<Response>()
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
