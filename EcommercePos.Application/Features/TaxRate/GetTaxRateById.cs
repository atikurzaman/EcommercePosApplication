using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.TaxRate;

public static class GetTaxRateById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string TaxName, decimal Rate, string? TaxCode, string? Description,
        bool IsActive, string TaxType, bool IsPercentage, bool IsInclusive, bool IsDefault,
        string? Country, bool ApplyToShipping, int Priority,
        DateTime? EffectiveFrom, DateTime? EffectiveTo);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.TaxRates
                .Where(x => x.Id == query.Id && !x.IsDeleted)
                .AsNoTracking()
                .Select(x => new Response(
                    x.Id, x.TaxName, x.Rate, x.TaxCode, x.Description, x.IsActive,
                    x.TaxType, x.IsPercentage, x.IsInclusive, x.IsDefault,
                    x.Country, x.ApplyToShipping, x.Priority,
                    x.EffectiveFrom, x.EffectiveTo))
                .FirstOrDefaultAsync(ct);

            return item is null
                ? Result<Response>.Failure(Error.NotFound($"Tax rate '{query.Id}' was not found."))
                : Result<Response>.Success(item);
        }
    }
}
