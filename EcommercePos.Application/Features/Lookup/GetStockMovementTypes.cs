using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetStockMovementTypes
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string TypeCode, string DisplayName, bool IsInbound);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.StockMovementTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.TypeCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.TypeCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.TypeCode, c.DisplayName, c.IsInbound))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
