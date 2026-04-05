using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class GoodsReceiptLines
{
    public Guid Id { get; set; }

    public Guid GoodsReceiptId { get; set; }

    public Guid PurchaseOrderLineId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual GoodsReceipts GoodsReceipt { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual PurchaseOrderLines PurchaseOrderLine { get; set; } = null!;

    public virtual ProductVariants? Variant { get; set; }
}
