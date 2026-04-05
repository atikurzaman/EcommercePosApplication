using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class PosTransactionBundleSelections
{
    public Guid Id { get; set; }

    public Guid PosTransactionLineId { get; set; }

    public Guid GroupId { get; set; }

    public Guid VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal PriceAdjustment { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual BundleOptionGroups Group { get; set; } = null!;

    public virtual PosTransactionLines PosTransactionLine { get; set; } = null!;

    public virtual ProductVariants Variant { get; set; } = null!;
}
