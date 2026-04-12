using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetSpecifications
{
    public sealed record Request(int PageIndex = 0, int PageSize = 50, string? Search = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string SpecName { get; init; } = string.Empty;
        public int SortOrder { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, string? Search);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.ProductSpecifications
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.Search))
                q = q.Where(x => x.SpecName.Contains(query.Search));

            var totalCount = await q.CountAsync(ct);

            var items = await q
                .OrderBy(x => x.SortOrder)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new Response
                {
                    Id = x.Id,
                    SpecName = x.SpecName,
                    SortOrder = x.SortOrder
                })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
