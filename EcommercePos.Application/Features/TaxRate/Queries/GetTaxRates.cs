using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.TaxRate.Queries;

public static class GetTaxRates
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string TaxName { get; init; } = string.Empty;
        public decimal TaxRate { get; init; }
        public string? TaxCode { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; }
    }

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
            var dbQuery = _context.TaxRates
                .Where(x => !x.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(x => x.TaxName.Contains(query.Search) || x.TaxCode.Contains(query.Search));
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderBy(x => x.TaxName)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .ProjectToType<Response>()
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}
