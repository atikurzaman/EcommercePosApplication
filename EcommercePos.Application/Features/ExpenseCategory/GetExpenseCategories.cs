using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ExpenseCategory;

public static class GetExpenseCategories
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(Guid Id, string Name, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.ExpenseCategories
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(c => c.Name.Contains(query.Search));

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(c => c.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new Response(c.Id, c.Name, c.Description, c.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
