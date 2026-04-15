using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Shipments
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid? ShippingMethodId { get; set; }

    public Guid? CarrierId { get; set; }

    public Guid? WarehouseId { get; set; }

    public string TrackingNumber { get; set; } = null!;

    public string? TrackingUrl { get; set; }

    public string StatusCode { get; set; } = null!;

    public DateTime? ShippedDate { get; set; }

    public DateTime? EstimatedDeliveryDate { get; set; }

    public DateTime? DeliveredDate { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal WeightKg { get; set; }

    public string? DeliveryNotes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ShippingCarriers? Carrier { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Orders Order { get; set; } = null!;

    public virtual ShippingMethods? ShippingMethod { get; set; }

    public virtual ShipmentStatuses StatusCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses? Warehouse { get; set; }
}
