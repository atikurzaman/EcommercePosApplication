using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.TaxRate.Queries;

public static class GetTaxRateById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
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
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var item = await _context.TaxRates
                .Where(x => x.Id == query.Id && !x.IsDeleted)
                .AsNoTracking()
                .ProjectToType<Response>()
                .FirstOrDefaultAsync(ct);

            if (item == null)
            {
                return Result<Response>.Failure(Error.NotFound($"TaxRate with id '{query.Id}' was not found."));
            }

            return Result<Response>.Success(item);
        }
    }
}
