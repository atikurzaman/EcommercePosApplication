using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class GoodsReceipts
{
    public Guid Id { get; set; }

    public Guid PurchaseOrderId { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid ReceivedByUserId { get; set; }

    public string ReceiptNumber { get; set; } = null!;

    public DateTime ReceiptDate { get; set; }

    public string? Condition { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<GoodsReceiptLines> GoodsReceiptLines { get; set; } = new List<GoodsReceiptLines>();

    public virtual PurchaseOrders PurchaseOrder { get; set; } = null!;

    public virtual Users ReceivedByUser { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
