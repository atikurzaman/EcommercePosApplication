using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", async (
            [AsParameters] GetOrdersRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Orders
                .Include(o => o.Customer).ThenInclude(c => c.User)
                .Include(o => o.StatusCodeNavigation)
                .Include(o => o.Warehouse)
                .Where(o => !o.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(o => o.OrderNumber.Contains(request.Search) || (o.Customer.Phone != null && o.Customer.Phone.Contains(request.Search)));

            if (!string.IsNullOrWhiteSpace(request.StatusCode))
                query = query.Where(o => o.StatusCode == request.StatusCode);

            if (!string.IsNullOrWhiteSpace(request.CustomerId))
                query = query.Where(o => o.CustomerId == Guid.Parse(request.CustomerId));

            if (!string.IsNullOrWhiteSpace(request.WarehouseId))
                query = query.Where(o => o.WarehouseId == Guid.Parse(request.WarehouseId));

            if (request.StartDate.HasValue)
                query = query.Where(o => o.OrderDate >= request.StartDate);

            if (request.EndDate.HasValue)
                query = query.Where(o => o.OrderDate <= request.EndDate);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrderListItemResponse(
                    o.Id, o.OrderNumber, o.CustomerId, 
                    o.Customer.User != null ? o.Customer.User.FirstName + " " + o.Customer.User.LastName : o.Customer.Phone,
                    o.Customer.Phone, o.WarehouseId, o.Warehouse != null ? o.Warehouse.Name : null,
                    o.StatusCode, o.StatusCodeNavigation.DisplayName, o.OrderDate,
                    o.TotalAmount, o.PaidAmount, o.RefundedAmount))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetOrders")
        .WithSummary("Get paginated orders");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var order = await context.Orders
                .Include(o => o.Customer).ThenInclude(c => c.User)
                .Include(o => o.StatusCodeNavigation)
                .Include(o => o.Warehouse)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .Include(o => o.Payments)
                .Include(o => o.Shipments)
                .Where(o => o.Id == id && !o.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (order == null)
                return Results.NotFound(new { error = "Order not found" });

            var customerName = order.Customer.User != null 
                ? order.Customer.User.FirstName + " " + order.Customer.User.LastName 
                : order.Customer.Phone;
            var customerEmail = order.Customer.User?.Email;

            var response = new OrderDetailResponse(
                order.Id, order.OrderNumber, order.CustomerId,
                customerName, order.Customer.Phone, customerEmail,
                order.WarehouseId, order.Warehouse != null ? order.Warehouse.Name : null,
                order.StatusCode, order.StatusCodeNavigation.DisplayName,
                order.OrderDate, order.OrderConfirmedDate, order.ShippedDate, order.DeliveredDate,
                order.CancellationDate, order.CancellationReason,
                order.SubTotal, order.ShippingAmount, order.TaxAmount, order.DiscountAmount,
                order.TotalAmount, order.PaidAmount, order.RefundedAmount,
                order.CustomerNote, order.AdminNote,
                new OrderAddressResponse(order.ShippingAddress.Id, order.ShippingAddress.AddressLine1,
                    order.ShippingAddress.City, order.ShippingAddress.PhoneNumber),
                order.BillingAddress != null ? new OrderAddressResponse(order.BillingAddress.Id,
                    order.BillingAddress.AddressLine1, order.BillingAddress.City, order.BillingAddress.PhoneNumber) : null,
                order.OrderItems.Select(i => new OrderLineResponse(
                    i.Id, i.ProductId, i.Product.Name, i.VariantId, i.VariantName,
                    i.Sku, i.Quantity, i.UnitPrice, i.DiscountAmount, i.TaxAmount, i.TotalPrice)).ToList(),
                order.Payments.Select(p => new OrderPaymentResponse(
                    p.Id, p.MethodCode, p.Amount, p.TransactionId, p.StatusCode, p.PaidAt)).ToList(),
                order.Shipments.Select(s => new OrderShipmentResponse(
                    s.Id, s.TrackingNumber, s.StatusCode, s.ShippedDate, s.DeliveredDate)).ToList());

            return Results.Ok(new { data = response });
        })
        .WithName("GetOrderById")
        .WithSummary("Get order with details");

        group.MapPut("/{id:guid}/status", async (Guid id, UpdateOrderStatusForOrderRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var order = await context.Orders.FindAsync(new object[] { id }, ct);
            if (order == null || order.IsDeleted)
                return Results.NotFound(new { error = "Order not found" });

            var oldStatus = order.StatusCode;
            order.StatusCode = request.StatusCode;
            order.UpdatedAt = DateTime.UtcNow;

            if (request.StatusCode == "CONFIRMED" && oldStatus == "PENDING")
                order.OrderConfirmedDate = DateTime.UtcNow;
            else if (request.StatusCode == "SHIPPED" && oldStatus == "CONFIRMED")
                order.ShippedDate = DateTime.UtcNow;
            else if (request.StatusCode == "DELIVERED" && oldStatus == "SHIPPED")
                order.DeliveredDate = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { order.Id, order.StatusCode } });
        })
        .WithName("UpdateOrderStatusById")
        .WithSummary("Update order status by ID");

        group.MapPost("/{id:guid}/cancel", async (Guid id, CancelOrderForReasonRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Id == id && !o.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (order == null)
                return Results.NotFound(new { error = "Order not found" });

            if (order.StatusCode == "DELIVERED" || order.StatusCode == "CANCELLED")
                return Results.BadRequest(new { error = "Order cannot be cancelled" });

            order.StatusCode = "CANCELLED";
            order.CancellationDate = DateTime.UtcNow;
            order.CancellationReason = request.Reason;
            order.UpdatedAt = DateTime.UtcNow;

            foreach (var item in order.OrderItems)
            {
                var stockItem = await context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.WarehouseId == order.WarehouseId && !s.IsDeleted, ct);

                if (stockItem != null)
                {
                    stockItem.QuantityOnHand += item.Quantity;
                    stockItem.UpdatedAt = DateTime.UtcNow;

                    context.StockMovements.Add(new StockMovements
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

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { order.Id, order.StatusCode } });
        })
        .WithName("CancelOrder")
        .WithSummary("Cancel order and restore stock");

        group.MapGet("/stats", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var today = DateTime.UtcNow.Date;
            var stats = new
            {
                TotalOrders = await context.Orders.Where(o => !o.IsDeleted).CountAsync(ct),
                PendingOrders = await context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "PENDING").CountAsync(ct),
                ProcessingOrders = await context.Orders.Where(o => !o.IsDeleted && (o.StatusCode == "CONFIRMED" || o.StatusCode == "PROCESSING")).CountAsync(ct),
                ShippedOrders = await context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "SHIPPED").CountAsync(ct),
                DeliveredOrders = await context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "DELIVERED").CountAsync(ct),
                CancelledOrders = await context.Orders.Where(o => !o.IsDeleted && o.StatusCode == "CANCELLED").CountAsync(ct),
                TodayOrders = await context.Orders.Where(o => !o.IsDeleted && o.OrderDate >= today).CountAsync(ct),
                TodaySales = await context.Orders.Where(o => !o.IsDeleted && o.OrderDate >= today).SumAsync(o => o.TotalAmount, ct)
            };

            return Results.Ok(new { data = stats });
        })
        .WithName("GetOrderStats")
        .WithSummary("Get order statistics");
    }
}

public record GetOrdersRequest(
    int PageIndex = 0, int PageSize = 20, string? Search = null,
    string? StatusCode = null, string? CustomerId = null, string? WarehouseId = null,
    DateTime? StartDate = null, DateTime? EndDate = null);

public record OrderListItemResponse(
    Guid Id, string OrderNumber, Guid CustomerId, string CustomerName, string CustomerPhone,
    Guid? WarehouseId, string? WarehouseName, string StatusCode, string StatusName,
    DateTime OrderDate, decimal TotalAmount, decimal PaidAmount, decimal RefundedAmount);

public record OrderDetailResponse(
    Guid Id, string OrderNumber, Guid CustomerId, string CustomerName, string CustomerPhone, string? CustomerEmail,
    Guid? WarehouseId, string? WarehouseName, string StatusCode, string StatusName,
    DateTime OrderDate, DateTime? OrderConfirmedDate, DateTime? ShippedDate, DateTime? DeliveredDate,
    DateTime? CancellationDate, string? CancellationReason,
    decimal SubTotal, decimal ShippingAmount, decimal TaxAmount, decimal DiscountAmount,
    decimal TotalAmount, decimal PaidAmount, decimal RefundedAmount,
    string? CustomerNote, string? AdminNote,
    OrderAddressResponse ShippingAddress, OrderAddressResponse? BillingAddress,
    List<OrderLineResponse> Items, List<OrderPaymentResponse> Payments, List<OrderShipmentResponse> Shipments);

public record OrderAddressResponse(Guid Id, string Address, string City, string? Phone);

public record OrderLineResponse(
    Guid Id, Guid ProductId, string ProductName, Guid? VariantId, string? VariantName,
    string? Sku, decimal Quantity, decimal UnitPrice, decimal DiscountAmount, decimal TaxAmount, decimal TotalPrice);

public record OrderPaymentResponse(
    Guid Id, string PaymentMethod, decimal Amount, string? TransactionId, string StatusCode, DateTime? PaidAt);

public record OrderShipmentResponse(
    Guid Id, string? TrackingNumber, string StatusCode, DateTime? ShippedDate, DateTime? DeliveredDate);

public record UpdateOrderStatusForOrderRequest(string StatusCode);

public record CancelOrderForReasonRequest(string Reason);