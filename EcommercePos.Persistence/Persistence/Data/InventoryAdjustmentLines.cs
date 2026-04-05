using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class InventoryAdjustmentLines
{
    public Guid Id { get; set; }

    public Guid InventoryAdjustmentId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public decimal SystemQuantity { get; set; }

    public decimal CountedQuantity { get; set; }

    public decimal AdjustmentQuantity { get; set; }

    public decimal UnitCost { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual InventoryAdjustments InventoryAdjustment { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual ProductVariants? Variant { get; set; }
}
