using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.TaxRate;

public static class CreateTaxRate
{
    public sealed record Command(
        string TaxName, decimal Rate, string? TaxCode, string? Description, bool IsActive,
        string TaxType, bool IsPercentage, bool IsInclusive, bool IsDefault, string Country,
        bool ApplyToShipping, int Priority, DateTime? EffectiveFrom, DateTime? EffectiveTo);

    public sealed record Response(Guid Id, string TaxName, decimal Rate, string? TaxCode,
        string? Description, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.TaxName).NotEmpty();
            RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TaxType).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(command.TaxCode))
            {
                var exists = await _context.TaxRates
                    .AnyAsync(x => x.TaxCode == command.TaxCode && !x.IsDeleted, ct);
                if (exists)
                    return Result<Response>.Failure(Error.Conflict($"Tax rate with code '{command.TaxCode}' already exists."));
            }

            var item = new TaxRates
            {
                Id = Guid.NewGuid(),
                TaxName = command.TaxName,
                Rate = command.Rate,
                TaxCode = command.TaxCode,
                Description = command.Description,
                IsActive = command.IsActive,
                TaxType = command.TaxType,
                IsPercentage = command.IsPercentage,
                IsInclusive = command.IsInclusive,
                IsDefault = command.IsDefault,
                Country = command.Country,
                ApplyToShipping = command.ApplyToShipping,
                Priority = command.Priority,
                EffectiveFrom = command.EffectiveFrom,
                EffectiveTo = command.EffectiveTo,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.TaxRates.Add(item);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                item.Id, item.TaxName, item.Rate, item.TaxCode, item.Description, item.IsActive));
        }
    }
}
