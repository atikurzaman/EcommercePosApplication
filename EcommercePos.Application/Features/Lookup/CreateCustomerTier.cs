using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class CreateCustomerTier
{
    public sealed record Request(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);
    public sealed record Response(string TierCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TierCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.CustomerTiers.AnyAsync(c => c.TierCode == request.TierCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Customer tier '{request.TierCode}' already exists."));

            var entity = new CustomerTiers
            {
                TierCode = request.TierCode,
                DisplayName = request.DisplayName,
                MinLifetimeSpend = request.MinLifetimeSpend,
                DiscountPct = request.DiscountPct,
                PointsMultiplier = request.PointsMultiplier,
                SortOrder = request.SortOrder
            };

            _context.CustomerTiers.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.TierCode, entity.DisplayName));
        }
    }
}
