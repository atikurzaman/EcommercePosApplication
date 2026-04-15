using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class PurchaseReturns : AuditableEntity<Guid>
{
    public Guid PurchaseOrderId { get; set; }

    public string ReturnNo { get; set; } = null!;

    public DateTime ReturnDate { get; set; }

    public Guid SupplierId { get; set; }

    public Guid WarehouseId { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? CreatedByUser { get; set; }

    public virtual PurchaseOrders PurchaseOrder { get; set; } = null!;

    public virtual ICollection<PurchaseReturnLines> PurchaseReturnLines { get; set; } = new List<PurchaseReturnLines>();

    public virtual Suppliers Supplier { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
