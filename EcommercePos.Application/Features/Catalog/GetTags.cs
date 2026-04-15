using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Catalog;

public static class GetTags
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public int ProductCount { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, string? Search);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.Tags.Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(query.Search))
                q = q.Where(x => x.Name.Contains(query.Search) || x.Slug.Contains(query.Search));

            var totalCount = await q.CountAsync(ct);

            var items = await q
                .OrderBy(x => x.Name)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new Response
                {
                    Id = x.Id,
                    Name = x.Name,
                    Slug = x.Slug,
                    ProductCount = x.ProductTags.Count
                })
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
