using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateCustomerTier
{
    public sealed record Request(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);
    public sealed record Command(string OriginalCode, string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetCustomerTierByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.CustomerTiers.FirstOrDefaultAsync(c => c.TierCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetCustomerTierByCode.Response>.Failure(Error.NotFound("Customer tier not found."));

            if (entity.TierCode != command.TierCode)
            {
                var exists = await _context.CustomerTiers.AnyAsync(c => c.TierCode == command.TierCode, ct);
                if (exists)
                    return Result<GetCustomerTierByCode.Response>.Failure(Error.Conflict($"Customer tier '{command.TierCode}' already exists."));
            }

            entity.TierCode = command.TierCode;
            entity.DisplayName = command.DisplayName;
            entity.MinLifetimeSpend = command.MinLifetimeSpend;
            entity.DiscountPct = command.DiscountPct;
            entity.PointsMultiplier = command.PointsMultiplier;
            entity.SortOrder = command.SortOrder;

            await _context.SaveChangesAsync(ct);
            return Result<GetCustomerTierByCode.Response>.Success(
                new GetCustomerTierByCode.Response(entity.TierCode, entity.DisplayName, entity.MinLifetimeSpend, entity.DiscountPct, entity.PointsMultiplier, entity.SortOrder));
        }
    }
}
