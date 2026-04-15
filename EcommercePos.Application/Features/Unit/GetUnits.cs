using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Unit;

public static class GetUnits
{
    public sealed record Query(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response(Guid Id, string ShortName, string Name, string? Description, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Units.Where(u => !u.IsDeleted).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(u =>
                    u.Name.Contains(query.Search) || u.ShortName.Contains(query.Search));

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(u => u.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(u => new Response(u.Id, u.ShortName, u.Name, u.Description, u.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
