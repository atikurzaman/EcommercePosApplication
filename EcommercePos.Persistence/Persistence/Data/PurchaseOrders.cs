using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class PurchaseOrders
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public Guid SupplierId { get; set; }

    public Guid? WarehouseId { get; set; }

    public string? InvoiceNo { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public DateTime? ActualDeliveryDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public decimal TransportCost { get; set; }

    public decimal OtherCost { get; set; }

    public decimal RoundOffAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal DueAmount { get; set; }

    public decimal TotalItemQuantity { get; set; }

    public string? Notes { get; set; }

    public string? ShippingAddress { get; set; }

    public string? BillingAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? ApprovedByUser { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users CreatedByUser { get; set; } = null!;

    public virtual ICollection<GoodsReceipts> GoodsReceipts { get; set; } = new List<GoodsReceipts>();

    public virtual ICollection<PurchaseOrderLines> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLines>();

    public virtual ICollection<PurchaseReturns> PurchaseReturns { get; set; } = new List<PurchaseReturns>();

    public virtual Suppliers Supplier { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses? Warehouse { get; set; }
}
