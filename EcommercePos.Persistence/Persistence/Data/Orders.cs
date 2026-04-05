using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class Orders
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? WarehouseId { get; set; }

    public Guid ShippingAddressId { get; set; }

    public Guid? BillingAddressId { get; set; }

    public Guid? AppliedDiscountId { get; set; }

    public string StatusCode { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public DateTime? OrderConfirmedDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public DateTime? DeliveredDate { get; set; }

    public DateTime? CancellationDate { get; set; }

    public string? CancellationReason { get; set; }

    public decimal SubTotal { get; set; }

    public decimal ShippingAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal RefundedAmount { get; set; }

    public string? CustomerNote { get; set; }

    public string? AdminNote { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Discounts? AppliedDiscount { get; set; }

    public virtual CustomerAddresses? BillingAddress { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual ICollection<DiscountUsageLog> DiscountUsageLog { get; set; } = new List<DiscountUsageLog>();

    public virtual ICollection<Invoices> Invoices { get; set; } = new List<Invoices>();

    public virtual ICollection<LoyaltyTransactions> LoyaltyTransactions { get; set; } = new List<LoyaltyTransactions>();

    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();

    public virtual ICollection<Quotes> Quotes { get; set; } = new List<Quotes>();

    public virtual ICollection<RefundRequests> RefundRequests { get; set; } = new List<RefundRequests>();

    public virtual ICollection<Returns> Returns { get; set; } = new List<Returns>();

    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();

    public virtual ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();

    public virtual CustomerAddresses ShippingAddress { get; set; } = null!;

    public virtual OrderStatuses StatusCodeNavigation { get; set; } = null!;

    public virtual ICollection<SupportTickets> SupportTickets { get; set; } = new List<SupportTickets>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }

    public virtual Warehouses? Warehouse { get; set; }
}
