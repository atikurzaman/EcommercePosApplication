using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.StockTransfer;

public static class ReceiveStockTransfer
{
    public sealed record Command(Guid Id);

    public sealed record Response(Guid Id, string Status);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var transfer = await _context.StockTransfers
                .Include(t => t.StockTransferLines)
                .Where(t => t.Id == command.Id && !t.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (transfer is null)
                return Result<Response>.Failure(Error.NotFound($"Stock transfer '{command.Id}' was not found."));

            if (transfer.Status == "RECEIVED")
                return Result<Response>.Failure(Error.Conflict("Transfer already received."));

            foreach (var line in transfer.StockTransferLines)
            {
                var toStockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == line.ProductId &&
                        s.WarehouseId == transfer.ToWarehouseId && !s.IsDeleted, ct);

                if (toStockItem is null)
                {
                    toStockItem = new StockItems
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        WarehouseId = transfer.ToWarehouseId,
                        QuantityOnHand = 0,
                        ReservedQuantity = 0,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    _context.StockItems.Add(toStockItem);
                }

                toStockItem.QuantityOnHand += line.Quantity;
                toStockItem.UpdatedAt = DateTime.UtcNow;

                _context.StockMovements.Add(new StockMovements
                {
                    Id = Guid.NewGuid(),
                    ProductId = line.ProductId,
                    VariantId = line.VariantId,
                    StockItemId = toStockItem.Id,
                    MovementTypeCode = "TRANSFER_IN",
                    QuantityIn = line.Quantity,
                    QuantityOut = 0,
                    BalanceAfter = toStockItem.QuantityOnHand,
                    ReferenceType = "StockTransfer",
                    ReferenceId = transfer.Id,
                    ReferenceNumber = transfer.TransferNo,
                    OccurredAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            transfer.Status = "RECEIVED";
            transfer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(transfer.Id, transfer.Status));
        }
    }
}
