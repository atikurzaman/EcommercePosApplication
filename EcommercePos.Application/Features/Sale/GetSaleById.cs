using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Sale;

public static class GetSaleById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string OrderNumber, DateTime OrderDate, decimal SubTotal, decimal DiscountAmount,
        decimal TaxAmount, decimal TotalAmount, decimal PaidAmount, decimal DueAmount,
        string StatusCode, decimal ShippingAmount, Guid CustomerId,
        Guid? WarehouseId, Guid? CreatedBy);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Id == query.Id && !o.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (order is null)
                return Result<Response>.Failure(Error.NotFound($"Sale '{query.Id}' was not found."));

            return Result<Response>.Success(new Response(
                order.Id, order.OrderNumber, order.OrderDate, order.SubTotal, order.DiscountAmount,
                order.TaxAmount, order.TotalAmount, order.PaidAmount, order.TotalAmount - order.PaidAmount,
                order.StatusCode, order.ShippingAmount, order.CustomerId,
                order.WarehouseId, order.CreatedBy));
        }
    }
}
