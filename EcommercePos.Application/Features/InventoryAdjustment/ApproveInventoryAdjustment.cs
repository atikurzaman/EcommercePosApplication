using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.InventoryAdjustment;

public static class ApproveInventoryAdjustment
{
    public sealed record Command(Guid Id, Guid ApprovedByUserId);

    public sealed record Response(Guid Id, DateTime ApprovedAt);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var adj = await _context.InventoryAdjustments
                .Include(a => a.InventoryAdjustmentLines)
                .Where(a => a.Id == command.Id && !a.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (adj is null)
                return Result<Response>.Failure(Error.NotFound($"Inventory adjustment '{command.Id}' was not found."));

            if (adj.ApprovedByUserId != null)
                return Result<Response>.Failure(Error.Conflict("Adjustment already approved."));

            adj.ApprovedByUserId = command.ApprovedByUserId;
            adj.ApprovedAt = DateTime.UtcNow;
            adj.UpdatedAt = DateTime.UtcNow;

            foreach (var line in adj.InventoryAdjustmentLines)
            {
                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == line.ProductId &&
                        s.WarehouseId == adj.WarehouseId && !s.IsDeleted, ct);

                if (stockItem != null)
                {
                    stockItem.QuantityOnHand += line.AdjustmentQuantity;
                    stockItem.UpdatedAt = DateTime.UtcNow;

                    _context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        StockItemId = stockItem.Id,
                        MovementTypeCode = adj.AdjustmentType == "INCREASE" ? "ADJ_IN" : "ADJ_OUT",
                        QuantityIn = adj.AdjustmentType == "INCREASE" ? line.AdjustmentQuantity : 0,
                        QuantityOut = adj.AdjustmentType == "DECREASE" ? line.AdjustmentQuantity : 0,
                        BalanceAfter = stockItem.QuantityOnHand,
                        ReferenceType = "InventoryAdjustment",
                        ReferenceId = adj.Id,
                        ReferenceNumber = adj.AdjustmentNo,
                        Notes = line.Remarks,
                        OccurredAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(adj.Id, adj.ApprovedAt!.Value));
        }
    }
}
