using System.ComponentModel;
using EcommercePos.Domain.Enums;

namespace EcommercePos.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }

    public static IEnumerable<OrderStatus> GetOrderStatuses() => 
        Enum.GetValues<OrderStatus>();

    public static IEnumerable<PaymentStatus> GetPaymentStatuses() => 
        Enum.GetValues<PaymentStatus>();

    public static IEnumerable<ShipmentStatus> GetShipmentStatuses() => 
        Enum.GetValues<ShipmentStatus>();

    public static IEnumerable<CustomerType> GetCustomerTypes() => 
        Enum.GetValues<CustomerType>();
}

public static class OrderStatusExtensions
{
    public static string ToDisplayString(this OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Pending",
        OrderStatus.Confirmed => "Confirmed",
        OrderStatus.Processing => "Processing",
        OrderStatus.Shipped => "Shipped",
        OrderStatus.Delivered => "Delivered",
        OrderStatus.Cancelled => "Cancelled",
        OrderStatus.Returned => "Returned",
        OrderStatus.OnHold => "On Hold",
        _ => status.ToString()
    };

    public static bool CanTransition(this OrderStatus current, OrderStatus newStatus) => (current, newStatus) switch
    {
        (OrderStatus.Pending, OrderStatus.Confirmed) => true,
        (OrderStatus.Pending, OrderStatus.Cancelled) => true,
        (OrderStatus.Confirmed, OrderStatus.Processing) => true,
        (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
        (OrderStatus.Processing, OrderStatus.Shipped) => true,
        (OrderStatus.Shipped, OrderStatus.Delivered) => true,
        (OrderStatus.Shipped, OrderStatus.Returned) => true,
        (OrderStatus.Delivered, OrderStatus.Returned) => true,
        _ => false
    };
}

public static class PaymentStatusExtensions
{
    public static string ToDisplayString(this PaymentStatus status) => status switch
    {
        PaymentStatus.Pending => "Pending",
        PaymentStatus.Processing => "Processing",
        PaymentStatus.Completed => "Completed",
        PaymentStatus.Failed => "Failed",
        PaymentStatus.Refunded => "Refunded",
        PaymentStatus.PartiallyRefunded => "Partially Refunded",
        PaymentStatus.Cancelled => "Cancelled",
        _ => status.ToString()
    };

    public static bool IsPaid(this PaymentStatus status) => status == PaymentStatus.Completed;
    public static bool IsPending(this PaymentStatus status) => status == PaymentStatus.Pending;
    public static bool CanRefund(this PaymentStatus status) => status == PaymentStatus.Completed;
}

public static class ShipmentStatusExtensions
{
    public static string ToDisplayString(this ShipmentStatus status) => status switch
    {
        ShipmentStatus.Pending => "Pending",
        ShipmentStatus.Packed => "Packed",
        ShipmentStatus.Shipped => "Shipped",
        ShipmentStatus.InTransit => "In Transit",
        ShipmentStatus.OutForDelivery => "Out for Delivery",
        ShipmentStatus.Delivered => "Delivered",
        ShipmentStatus.FailedDelivery => "Failed Delivery",
        ShipmentStatus.Returned => "Returned",
        ShipmentStatus.Cancelled => "Cancelled",
        _ => status.ToString()
    };
}

public static class CustomerTypeExtensions
{
    public static string ToDisplayString(this CustomerType type) => type switch
    {
        CustomerType.Individual => "Individual",
        CustomerType.Business => "Business",
        CustomerType.Wholesale => "Wholesale",
        CustomerType.VIP => "VIP",
        _ => type.ToString()
    };
}
