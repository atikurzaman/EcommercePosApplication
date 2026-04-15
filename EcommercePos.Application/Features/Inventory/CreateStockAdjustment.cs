using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class CreateStockAdjustment
{
    public sealed record Request
    {
        public Guid StockItemId { get; init; }
        public decimal Quantity { get; init; }
        public string AdjustmentType { get; init; } = string.Empty;
        public string? Reason { get; init; }
        public string? Notes { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public decimal NewQuantity { get; init; }
    }

    public sealed record Command(Request Request, Guid UserId);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var stockItem = await _context.StockItems
                .Where(s => s.Id == command.Request.StockItemId && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (stockItem == null)
                return Result<Response>.Failure(Error.NotFound("Stock item not found"));

            var qtyBefore = stockItem.QuantityOnHand;
            decimal qtyChange = 0;

            switch (command.Request.AdjustmentType.ToUpper())
            {
                case "ADD":
                    qtyChange = command.Request.Quantity;
                    stockItem.QuantityOnHand += command.Request.Quantity;
                    break;
                case "REMOVE":
                    qtyChange = -command.Request.Quantity;
                    if (stockItem.QuantityOnHand < command.Request.Quantity)
                        return Result<Response>.Failure(Error.Conflict("Insufficient stock"));
                    stockItem.QuantityOnHand -= command.Request.Quantity;
                    break;
                case "SET":
                    qtyChange = command.Request.Quantity - stockItem.QuantityOnHand;
                    stockItem.QuantityOnHand = command.Request.Quantity;
                    break;
                default:
                    return Result<Response>.Failure(Error.BadRequest("Invalid adjustment type"));
            }

            var movement = new StockMovements
            {
                Id = Guid.NewGuid(),
                StockItemId = stockItem.Id,
                ProductId = stockItem.ProductId,
                MovementTypeCode = command.Request.AdjustmentType.ToUpper(),
                QuantityIn = command.Request.AdjustmentType.ToUpper() == "ADD" ? Math.Abs(qtyChange) : 0,
                QuantityOut = command.Request.AdjustmentType.ToUpper() == "REMOVE" ? Math.Abs(qtyChange) : 0,
                BalanceAfter = stockItem.QuantityOnHand,
                ReferenceNumber = command.Request.Reason,
                Notes = command.Request.Notes,
                CreatedBy = command.UserId,
                CreatedAt = DateTime.Now
            };

            stockItem.LastUpdatedAt = DateTime.Now;
            _context.StockMovements.Add(movement);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = movement.Id,
                NewQuantity = stockItem.QuantityOnHand
            });
        }
    }
}
