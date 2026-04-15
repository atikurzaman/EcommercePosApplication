using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Sale;

public static class UpdateSale
{
    public sealed record Command(
        Guid Id, decimal SubTotal, decimal DiscountAmount, decimal TaxAmount,
        decimal TotalAmount, decimal PaidAmount, string StatusCode);

    public sealed record Response(Guid Id, string OrderNumber, decimal TotalAmount, string StatusCode);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var order = await _context.Orders
                .Where(o => o.Id == command.Id && !o.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (order is null)
                return Result<Response>.Failure(Error.NotFound($"Sale '{command.Id}' was not found."));

            order.SubTotal = command.SubTotal;
            order.DiscountAmount = command.DiscountAmount;
            order.TaxAmount = command.TaxAmount;
            order.TotalAmount = command.TotalAmount;
            order.PaidAmount = command.PaidAmount;
            order.StatusCode = command.StatusCode;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(
                order.Id, order.OrderNumber, order.TotalAmount, order.StatusCode));
        }
    }
}
