using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.StockTransfer;

public static class CreateStockTransfer
{
    public sealed record LineInput(Guid ProductId, Guid? VariantId, decimal Quantity, decimal UnitCost);

    public sealed record Command(
        Guid FromWarehouseId, Guid ToWarehouseId, string? Notes,
        List<LineInput> Lines);

    public sealed record Response(Guid Id, string TransferNo);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FromWarehouseId).NotEmpty();
            RuleFor(x => x.ToWarehouseId).NotEmpty();
            RuleFor(x => x.Lines).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var transferNo = $"TRF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

            var transfer = new StockTransfers
            {
                Id = Guid.NewGuid(),
                TransferNo = transferNo,
                FromWarehouseId = command.FromWarehouseId,
                ToWarehouseId = command.ToWarehouseId,
                TransferDate = DateTime.UtcNow,
                Status = "PENDING",
                Notes = command.Notes,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.StockTransfers.Add(transfer);

            foreach (var line in command.Lines)
            {
                _context.StockTransferLines.Add(new StockTransferLines
                {
                    Id = Guid.NewGuid(),
                    TransferId = transfer.Id,
                    ProductId = line.ProductId,
                    VariantId = line.VariantId,
                    Quantity = line.Quantity
                });

                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == line.ProductId &&
                        s.WarehouseId == command.FromWarehouseId && !s.IsDeleted, ct);

                if (stockItem != null && stockItem.QuantityOnHand >= line.Quantity)
                {
                    stockItem.QuantityOnHand -= line.Quantity;
                    stockItem.UpdatedAt = DateTime.UtcNow;

                    _context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = line.ProductId,
                        VariantId = line.VariantId,
                        StockItemId = stockItem.Id,
                        MovementTypeCode = "TRANSFER_OUT",
                        QuantityIn = 0,
                        QuantityOut = line.Quantity,
                        BalanceAfter = stockItem.QuantityOnHand,
                        ReferenceType = "StockTransfer",
                        ReferenceId = transfer.Id,
                        ReferenceNumber = transferNo,
                        OccurredAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(transfer.Id, transfer.TransferNo));
        }
    }
}
