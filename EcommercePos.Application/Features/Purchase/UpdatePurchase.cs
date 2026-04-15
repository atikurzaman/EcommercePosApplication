using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Purchase;

public static class UpdatePurchase
{
    public sealed record Command(
        Guid Id, string Status, decimal SubTotal, decimal DiscountAmount,
        decimal TotalTaxAmount, decimal GrandTotal);

    public sealed record Response(Guid Id, string OrderNumber, decimal GrandTotal, string Status);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var purchase = await _context.PurchaseOrders
                .Where(p => p.Id == command.Id && !p.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (purchase is null)
                return Result<Response>.Failure(Error.NotFound($"Purchase '{command.Id}' was not found."));

            purchase.Status = command.Status;
            purchase.SubTotal = command.SubTotal;
            purchase.DiscountAmount = command.DiscountAmount;
            purchase.TotalTaxAmount = command.TotalTaxAmount;
            purchase.GrandTotal = command.GrandTotal;
            purchase.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                purchase.Id, purchase.OrderNumber, purchase.GrandTotal, purchase.Status));
        }
    }
}
