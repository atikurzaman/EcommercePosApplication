using FluentValidation;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ShippingMethod;

public static class CreateShippingMethod
{
    public sealed record Command(
        string Name, string? Description, string? CarrierName, decimal BaseCost,
        decimal CostPerKg, int EstimatedDaysMin, int EstimatedDaysMax, bool IsActive, bool IsFreeShipping,
        decimal? FreeShippingThreshold, int DisplayOrder);

    public sealed record Response(Guid Id, string Name, decimal BaseCost, bool IsActive);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.BaseCost).GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var method = new ShippingMethods
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                CarrierName = command.CarrierName,
                BaseCost = command.BaseCost,
                CostPerKg = command.CostPerKg,
                EstimatedDaysMin = command.EstimatedDaysMin,
                EstimatedDaysMax = command.EstimatedDaysMax,
                IsActive = command.IsActive,
                IsFreeShipping = command.IsFreeShipping,
                FreeShippingThreshold = command.FreeShippingThreshold,
                DisplayOrder = command.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ShippingMethods.Add(method);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(method.Id, method.Name, method.BaseCost, method.IsActive));
        }
    }
}
