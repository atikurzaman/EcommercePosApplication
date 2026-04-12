using FluentValidation;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Purchase;

public static class CreatePurchase
{
    public sealed record Command(
        decimal SubTotal, decimal DiscountAmount, decimal TotalTaxAmount, decimal GrandTotal,
        Guid SupplierId, Guid? WarehouseId);

    public sealed record Response(Guid Id, string OrderNumber, decimal GrandTotal, string Status);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.SupplierId).NotEmpty();
            RuleFor(x => x.GrandTotal).GreaterThan(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var orderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            var purchase = new PurchaseOrders
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                SubTotal = command.SubTotal,
                DiscountAmount = command.DiscountAmount,
                TotalTaxAmount = command.TotalTaxAmount,
                GrandTotal = command.GrandTotal,
                Status = "PENDING",
                SupplierId = command.SupplierId,
                WarehouseId = command.WarehouseId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.PurchaseOrders.Add(purchase);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                purchase.Id, purchase.OrderNumber, purchase.GrandTotal, purchase.Status));
        }
    }
}
