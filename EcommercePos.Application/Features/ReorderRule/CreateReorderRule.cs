using FluentValidation;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.ReorderRule;

public static class CreateReorderRule
{
    public sealed record Command(
        Guid ProductId, Guid? VariantId, Guid? WarehouseId,
        Guid? PreferredSupplierId, decimal ReorderLevel, decimal ReorderQuantity,
        Guid? NotifyUserId);

    public sealed record Response(Guid Id);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ReorderQuantity).GreaterThan(0);
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var rule = new ReorderRules
            {
                Id = Guid.NewGuid(),
                ProductId = command.ProductId,
                VariantId = command.VariantId,
                WarehouseId = command.WarehouseId,
                PreferredSupplierId = command.PreferredSupplierId,
                ReorderLevel = command.ReorderLevel,
                ReorderQuantity = command.ReorderQuantity,
                NotifyUserId = command.NotifyUserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ReorderRules.Add(rule);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(rule.Id));
        }
    }
}
