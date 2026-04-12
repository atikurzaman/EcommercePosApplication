using FluentValidation;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Sale;

public static class CreateSale
{
    public sealed record Command(
        decimal SubTotal, decimal DiscountAmount, decimal TaxAmount, decimal TotalAmount,
        decimal PaidAmount, Guid CustomerId, Guid? WarehouseId);

    public sealed record Response(Guid Id, string OrderNumber, decimal TotalAmount, string StatusCode);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.TotalAmount).GreaterThan(0);
            RuleFor(x => x.CustomerId).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var orderNumber = $"SL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            var order = new Orders
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                SubTotal = command.SubTotal,
                DiscountAmount = command.DiscountAmount,
                TaxAmount = command.TaxAmount,
                TotalAmount = command.TotalAmount,
                PaidAmount = command.PaidAmount,
                StatusCode = "PENDING",
                CustomerId = command.CustomerId,
                WarehouseId = command.WarehouseId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                order.Id, order.OrderNumber, order.TotalAmount, order.StatusCode));
        }
    }
}
