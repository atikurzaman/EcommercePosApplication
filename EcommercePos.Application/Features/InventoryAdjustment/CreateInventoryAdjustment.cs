using FluentValidation;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.InventoryAdjustment;

public static class CreateInventoryAdjustment
{
    public sealed record LineInput(Guid ProductId, Guid? VariantId, decimal QuantityAdjusted, string Reason);

    public sealed record Command(
        Guid WarehouseId, string AdjustmentType, string Reason, string? Notes,
        List<LineInput> Lines);

    public sealed record Response(Guid Id, string AdjustmentNo);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.AdjustmentType).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty();
            RuleFor(x => x.Lines).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var adjNo = $"ADJ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

            var adj = new InventoryAdjustments
            {
                Id = Guid.NewGuid(),
                AdjustmentNo = adjNo,
                WarehouseId = command.WarehouseId,
                AdjustmentDate = DateTime.UtcNow,
                AdjustmentType = command.AdjustmentType,
                Reason = command.Reason,
                Notes = command.Notes,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.InventoryAdjustments.Add(adj);

            foreach (var line in command.Lines)
            {
                _context.InventoryAdjustmentLines.Add(new InventoryAdjustmentLines
                {
                    Id = Guid.NewGuid(),
                    InventoryAdjustmentId = adj.Id,
                    ProductId = line.ProductId,
                    VariantId = line.VariantId,
                    AdjustmentQuantity = line.QuantityAdjusted,
                    Remarks = line.Reason
                });
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(adj.Id, adj.AdjustmentNo));
        }
    }
}
