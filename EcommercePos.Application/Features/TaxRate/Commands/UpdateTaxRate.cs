using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.TaxRate.Commands;

public static class UpdateTaxRate
{
    public sealed record Request
    {
        public string TaxName { get; init; } = string.Empty;
        public decimal TaxRate { get; init; }
        public string? TaxCode { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; }
        public string TaxType { get; init; } = string.Empty;
        public bool IsPercentage { get; init; }
        public bool IsInclusive { get; init; }
        public bool IsDefault { get; init; }
        public string Country { get; init; } = string.Empty;
        public bool ApplyToShipping { get; init; }
        public int Priority { get; init; }
        public DateTime? EffectiveFrom { get; init; }
        public DateTime? EffectiveTo { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string TaxName { get; init; } = string.Empty;
        public decimal TaxRate { get; init; }
        public string? TaxCode { get; init; }
        public string? Description { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record Command(
        Guid Id, string TaxName, decimal TaxRate, string? TaxCode, string? Description, bool IsActive,
        string TaxType, bool IsPercentage, bool IsInclusive, bool IsDefault, string Country,
        bool ApplyToShipping, int Priority, DateTime? EffectiveFrom, DateTime? EffectiveTo);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator() {
            RuleFor(x => x.TaxName).NotEmpty();
            RuleFor(x => x.TaxRate).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TaxType).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var item = await _context.TaxRates
                .Where(x => x.Id == command.Id && !x.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"TaxRate with id '{command.Id}' was not found."));
            }

            if (!string.IsNullOrEmpty(command.TaxCode))
            {
                var exists = await _context.TaxRates
                    .AnyAsync(x => x.TaxCode == command.TaxCode && x.Id != command.Id && !x.IsDeleted, ct);

                if (exists)
                {
                    return Result<Response>.Failure(Error.Conflict($"Another TaxRate with TaxCode '{command.TaxCode}' already exists."));
                }
            }

            item.TaxName = command.TaxName;
            item.Rate = command.TaxRate;
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

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}
