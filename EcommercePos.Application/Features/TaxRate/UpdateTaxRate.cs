using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.TaxRate;

public static class UpdateTaxRate
{
    public sealed record Command(
        Guid Id, string TaxName, decimal Rate, string? TaxCode, string? Description, bool IsActive,
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
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.TaxRates
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item is null)
                return Result<Response>.Failure(Error.NotFound($"Tax rate '{command.Id}' was not found."));

            item.TaxName = command.TaxName;
            item.Rate = command.Rate;
            item.TaxCode = command.TaxCode;
            item.Description = command.Description;
            item.IsActive = command.IsActive;
            item.TaxType = command.TaxType;
            item.IsPercentage = command.IsPercentage;
            item.IsInclusive = command.IsInclusive;
            item.IsDefault = command.IsDefault;
            item.Country = command.Country;
            item.ApplyToShipping = command.ApplyToShipping;
            item.Priority = command.Priority;
            item.EffectiveFrom = command.EffectiveFrom;
            item.EffectiveTo = command.EffectiveTo;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(
                item.Id, item.TaxName, item.Rate, item.TaxCode, item.Description, item.IsActive));
        }
    }
}
