using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class GoodsReceipts : AuditableEntity<Guid>
{
    public Guid PurchaseOrderId { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid ReceivedByUserId { get; set; }

    public string ReceiptNumber { get; set; } = null!;

    public DateTime ReceiptDate { get; set; }

    public string? Condition { get; set; }

    public string? Notes { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<GoodsReceiptLines> GoodsReceiptLines { get; set; } = new List<GoodsReceiptLines>();

    public virtual PurchaseOrders PurchaseOrder { get; set; } = null!;

    public virtual Users ReceivedByUser { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
