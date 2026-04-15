using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ShippingMethod;

public static class UpdateShippingMethod
{
    public sealed record Command(
        Guid Id, string Name, string? Description, string? CarrierName, decimal BaseCost,
        decimal CostPerKg, int EstimatedDaysMin, int EstimatedDaysMax, bool IsActive, bool IsFreeShipping,
        decimal? FreeShippingThreshold, int DisplayOrder);

    public sealed record Response(Guid Id, string Name, decimal BaseCost, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator() => RuleFor(x => x.Name).NotEmpty();
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var method = await _context.ShippingMethods
                .Where(s => s.Id == command.Id && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (method is null)
                return Result<Response>.Failure(Error.NotFound($"Shipping method '{command.Id}' was not found."));

            method.Name = command.Name;
            method.Description = command.Description;
            method.CarrierName = command.CarrierName;
            method.BaseCost = command.BaseCost;
            method.CostPerKg = command.CostPerKg;
            method.EstimatedDaysMin = command.EstimatedDaysMin;
            method.EstimatedDaysMax = command.EstimatedDaysMax;
            method.IsActive = command.IsActive;
            method.IsFreeShipping = command.IsFreeShipping;
            method.FreeShippingThreshold = command.FreeShippingThreshold;
            method.DisplayOrder = command.DisplayOrder;
            method.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(method.Id, method.Name, method.BaseCost, method.IsActive));
        }
    }
}
