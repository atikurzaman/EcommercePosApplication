using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetCustomerTierByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.CustomerTiers.AsNoTracking()
                .Where(c => c.TierCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Customer tier not found."));

            return Result<Response>.Success(new Response(entity.TierCode, entity.DisplayName, entity.MinLifetimeSpend, entity.DiscountPct, entity.PointsMultiplier, entity.SortOrder));
        }
    }
}
