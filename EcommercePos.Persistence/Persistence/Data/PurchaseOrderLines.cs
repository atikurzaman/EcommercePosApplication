using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class PurchaseOrderLines
{
    public Guid Id { get; set; }

    public Guid PurchaseOrderId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Sku { get; set; }

    public decimal Quantity { get; set; }

    public decimal ReceivedQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal LineTotal { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<GoodsReceiptLines> GoodsReceiptLines { get; set; } = new List<GoodsReceiptLines>();

    public virtual Products Product { get; set; } = null!;

    public virtual PurchaseOrders PurchaseOrder { get; set; } = null!;

    public virtual ICollection<PurchaseOrderLineTaxes> PurchaseOrderLineTaxes { get; set; } = new List<PurchaseOrderLineTaxes>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
