using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetCurrencies
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Currencies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.Name.Contains(request.Search) || c.CurrencyCode.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.CurrencyCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.CurrencyCode, c.Name, c.Symbol, c.ExchangeRate, c.DecimalPlaces, c.IsBaseCurrency, c.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}
