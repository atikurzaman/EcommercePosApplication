using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ShippingMethod;

public static class GetShippingMethodById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string Name, string? Description, string? CarrierName, decimal BaseCost,
        decimal CostPerKg, int EstimatedDaysMin, int EstimatedDaysMax, bool IsActive, bool IsFreeShipping,
        decimal? FreeShippingThreshold, int DisplayOrder);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var method = await _context.ShippingMethods
                .Where(s => s.Id == query.Id && !s.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (method is null)
                return Result<Response>.Failure(Error.NotFound($"Shipping method '{query.Id}' was not found."));

            return Result<Response>.Success(new Response(
                method.Id, method.Name, method.Description, method.CarrierName, method.BaseCost,
                method.CostPerKg, method.EstimatedDaysMin, method.EstimatedDaysMax, method.IsActive, method.IsFreeShipping,
                method.FreeShippingThreshold, method.DisplayOrder));
        }
    }
}
