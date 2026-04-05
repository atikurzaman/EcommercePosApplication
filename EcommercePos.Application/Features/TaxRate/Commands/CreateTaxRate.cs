using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.TaxRate.Commands;

public static class CreateTaxRate
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
        string TaxName, decimal TaxRate, string? TaxCode, string? Description, bool IsActive,
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
            if (!string.IsNullOrEmpty(command.TaxCode))
            {
                var exists = await _context.TaxRates
                    .AnyAsync(x => x.TaxCode == command.TaxCode && !x.IsDeleted, ct);

                if (exists)
                {
                    return Result<Response>.Failure(Error.Conflict($"TaxRate with TaxCode '{command.TaxCode}' already exists."));
                }
            }

            var item = new TaxRates
            {
                Id = Guid.NewGuid(),
                TaxName = command.TaxName,
                Rate = command.TaxRate,
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

            return Result<Response>.Success(item.Adapt<Response>());
        }
    }
}
