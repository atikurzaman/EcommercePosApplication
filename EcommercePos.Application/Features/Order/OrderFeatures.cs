using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Order;

public static class GetOrders
{
    public sealed record Query(
        int PageIndex = 0,
        int PageSize = 20,
        string? Search = null,
        string? StatusCode = null,
        Guid? CustomerId = null,
        Guid? WarehouseId = null,
        DateTime? StartDate = null,
        DateTime? EndDate = null);

    public sealed record Response(
        Guid Id, string OrderNumber, Guid CustomerId, string CustomerName, string CustomerPhone,
        Guid? WarehouseId, string? WarehouseName, string StatusCode, string StatusName,
        DateTime OrderDate, decimal TotalAmount, decimal PaidAmount, decimal RefundedAmount);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.Orders
                .Include(o => o.Customer).ThenInclude(c => c.User)
                .Include(o => o.StatusCodeNavigation)
                .Include(o => o.Warehouse)
                .Where(o => !o.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
                dbQuery = dbQuery.Where(o => o.OrderNumber.Contains(query.Search) || (o.Customer.Phone != null && o.Customer.Phone.Contains(query.Search)));

            if (!string.IsNullOrWhiteSpace(query.StatusCode))
                dbQuery = dbQuery.Where(o => o.StatusCode == query.StatusCode);

            if (query.CustomerId.HasValue)
                dbQuery = dbQuery.Where(o => o.CustomerId == query.CustomerId.Value);

            if (query.WarehouseId.HasValue)
                dbQuery = dbQuery.Where(o => o.WarehouseId == query.WarehouseId.Value);

            if (query.StartDate.HasValue)
                dbQuery = dbQuery.Where(o => o.OrderDate >= query.StartDate.Value);

            if (query.EndDate.HasValue)
                dbQuery = dbQuery.Where(o => o.OrderDate <= query.EndDate.Value);

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .OrderByDescending(o => o.OrderDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(o => new Response(
                    o.Id, o.OrderNumber, o.CustomerId,
                    o.Customer.User != null ? o.Customer.User.FirstName + " " + o.Customer.User.LastName : (o.Customer.Phone ?? string.Empty),
                    o.Customer.Phone ?? string.Empty,
                    o.WarehouseId, o.Warehouse != null ? o.Warehouse.Name : null,
                    o.StatusCode, o.StatusCodeNavigation.DisplayName, o.OrderDate,
                    o.TotalAmount, o.PaidAmount, o.RefundedAmount))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}

public static class GetOrderById
{
    public sealed record Query(Guid Id);

    public sealed record AddressResponse(Guid Id, string Address, string City, string? Phone);

    public sealed record LineResponse(
        Guid Id, Guid ProductId, string ProductName, Guid? VariantId, string? VariantName,
        string? Sku, decimal Quantity, decimal UnitPrice, decimal DiscountAmount, decimal TaxAmount, decimal TotalPrice);

    public sealed record PaymentResponse(
        Guid Id, string PaymentMethod, decimal Amount, string? TransactionId, string StatusCode, DateTime? PaidAt);

    public sealed record ShipmentResponse(
        Guid Id, string? TrackingNumber, string StatusCode, DateTime? ShippedDate, DateTime? DeliveredDate);

    public sealed record Response(
        Guid Id, string OrderNumber, Guid CustomerId, string CustomerName, string CustomerPhone, string? CustomerEmail,
        Guid? WarehouseId, string? WarehouseName, string StatusCode, string StatusName,
        DateTime OrderDate, DateTime? OrderConfirmedDate, DateTime? ShippedDate, DateTime? DeliveredDate,
        DateTime? CancellationDate, string? CancellationReason,
        decimal SubTotal, decimal ShippingAmount, decimal TaxAmount, decimal DiscountAmount,
        decimal TotalAmount, decimal PaidAmount, decimal RefundedAmount,
        string? CustomerNote, string? AdminNote,
        AddressResponse ShippingAddress, AddressResponse? BillingAddress,
        List<LineResponse> Items, List<PaymentResponse> Payments, List<ShipmentResponse> Shipments);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var order = await _context.Orders
                .Include(o => o.Customer).ThenInclude(c => c.User)
                .Include(o => o.StatusCodeNavigation)
                .Include(o => o.Warehouse)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .Include(o => o.Payments)
                .Include(o => o.Shipments)
                .Where(o => o.Id == query.Id && !o.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (order == null)
                return Result<Response>.Failure(Error.NotFound("Order not found"));

            var customerName = order.Customer.User != null
                ? order.Customer.User.FirstName + " " + order.Customer.User.LastName
                : (order.Customer.Phone ?? string.Empty);
            var customerEmail = order.Customer.User?.Email;

            var response = new Response(
                order.Id, order.OrderNumber, order.CustomerId,
                customerName, order.Customer.Phone ?? string.Empty, customerEmail,
                order.WarehouseId, order.Warehouse != null ? order.Warehouse.Name : null,
                order.StatusCode, order.StatusCodeNavigation.DisplayName,
                order.OrderDate, order.OrderConfirmedDate, order.ShippedDate, order.DeliveredDate,
                order.CancellationDate, order.CancellationReason,
                order.SubTotal, order.ShippingAmount, order.TaxAmount, order.DiscountAmount,
                order.TotalAmount, order.PaidAmount, order.RefundedAmount,
                order.CustomerNote, order.AdminNote,
                new AddressResponse(order.ShippingAddress.Id, order.ShippingAddress.AddressLine1,
                    order.ShippingAddress.City, order.ShippingAddress.PhoneNumber),
                order.BillingAddress != null ? new AddressResponse(order.BillingAddress.Id,
                    order.BillingAddress.AddressLine1, order.BillingAddress.City, order.BillingAddress.PhoneNumber) : null,
                order.OrderItems.Select(i => new LineResponse(
                    i.Id, i.ProductId, i.Product.Name, i.VariantId, i.VariantName,
                    i.Sku, i.Quantity, i.UnitPrice, i.DiscountAmount, i.TaxAmount, i.TotalPrice)).ToList(),
                order.Payments.Select(p => new PaymentResponse(
                    p.Id, p.MethodCode, p.Amount, p.TransactionId, p.StatusCode, p.PaidAt)).ToList(),
                order.Shipments.Select(s => new ShipmentResponse(
                    s.Id, s.TrackingNumber, s.StatusCode, s.ShippedDate, s.DeliveredDate)).ToList());

            return Result<Response>.Success(response);
        }
    }
}

public static class UpdateOrderStatusById
{
    public sealed record Command(Guid Id, string StatusCode);
    public sealed record Response(Guid Id, string StatusCode);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var order = await _context.Orders.FindAsync(new object[] { command.Id }, ct);
            if (order == null || order.IsDeleted)
                return Result<Response>.Failure(Error.NotFound("Order not found"));

            var oldStatus = order.StatusCode;
            order.StatusCode = command.StatusCode;
            order.UpdatedAt = DateTime.UtcNow;

            if (command.StatusCode == "CONFIRMED" && oldStatus == "PENDING")
                order.OrderConfirmedDate = DateTime.UtcNow;
            else if (command.StatusCode == "SHIPPED" && oldStatus == "CONFIRMED")
                order.ShippedDate = DateTime.UtcNow;
            else if (command.StatusCode == "DELIVERED" && oldStatus == "SHIPPED")
                order.DeliveredDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(order.Id, order.StatusCode));
        }
    }
}

public static class CancelOrder
{
    public sealed record Command(Guid Id, string Reason);
    public sealed record Response(Guid Id, string StatusCode);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Id == command.Id && !o.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (order == null)
                return Result<Response>.Failure(Error.NotFound("Order not found"));

            if (order.StatusCode == "DELIVERED" || order.StatusCode == "CANCELLED")
                return Result<Response>.Failure(Error.Validation("Order cannot be cancelled"));

            order.StatusCode = "CANCELLED";
            order.CancellationDate = DateTime.UtcNow;
            order.CancellationReason = command.Reason;
            order.UpdatedAt = DateTime.UtcNow;

            foreach (var item in order.OrderItems)
            {
                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.WarehouseId == order.WarehouseId && !s.IsDeleted, ct);

                if (stockItem != null)
                {
                    stockItem.QuantityOnHand += item.Quantity;
                    stockItem.UpdatedAt = DateTime.UtcNow;

                    _context.StockMovements.Add(new StockMovements
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        StockItemId = stockItem.Id,
                        MovementTypeCode = "ORDER_CANCEL",
                        QuantityIn = item.Quantity,
                        QuantityOut = 0,
                        BalanceAfter = stockItem.QuantityOnHand,
                        ReferenceType = "Order",
                        ReferenceId = order.Id,
                        ReferenceNumber = order.OrderNumber,
                        Notes = "Order cancelled - stock returned",
                        OccurredAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _context.SaveChangesAsync(ct);
            return Result<Response>.Success(new Response(order.Id, order.StatusCode));
        }
    }
}

public static class GetOrderStats
{
    public sealed record Response(
        int TotalOrders,
        int PendingOrders,
        int ProcessingOrders,
        int ShippedOrders,
        int DeliveredOrders,
        int CancelledOrders,
        int TodayOrders,
        decimal TodaySales);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;

            var response = new Response(
                await _context.Orders.Where(o => !o.IsDeleted).CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "PENDING").CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && (o.StatusCode == "CONFIRMED" || o.StatusCode == "PROCESSING")).CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "SHIPPED").CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "DELIVERED").CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "CANCELLED").CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.OrderDate >= today).CountAsync(ct),
                await _context.Orders.Where(o => !o.IsDeleted && o.OrderDate >= today).SumAsync(o => o.TotalAmount, ct));

            return Result<Response>.Success(response);
        }
    }
}
