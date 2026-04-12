using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetCurrencyByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Currencies.AsNoTracking()
                .Where(c => c.CurrencyCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Currency not found."));

            return Result<Response>.Success(new Response(entity.CurrencyCode, entity.Name, entity.Symbol, entity.ExchangeRate, entity.DecimalPlaces, entity.IsBaseCurrency, entity.IsActive));
        }
    }
}
