using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Purchase;

public static class GetPurchaseById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, string OrderNumber, DateTime OrderDate, decimal SubTotal, decimal DiscountAmount,
        decimal TotalTaxAmount, decimal GrandTotal, string Status, Guid SupplierId,
        Guid? WarehouseId, Guid? CreatedBy);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var purchase = await _context.PurchaseOrders
                .Include(p => p.PurchaseOrderLines)
                .Where(p => p.Id == query.Id && !p.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (purchase is null)
                return Result<Response>.Failure(Error.NotFound($"Purchase '{query.Id}' was not found."));

            return Result<Response>.Success(new Response(
                purchase.Id, purchase.OrderNumber, purchase.OrderDate, purchase.SubTotal,
                purchase.DiscountAmount, purchase.TotalTaxAmount, purchase.GrandTotal, purchase.Status,
                purchase.SupplierId, purchase.WarehouseId, purchase.CreatedBy));
        }
    }
}
