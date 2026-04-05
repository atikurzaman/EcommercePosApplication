using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class BundleOptionItems
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }

    public Guid VariantId { get; set; }

    public decimal PriceAdjustment { get; set; }

    public bool IsDefault { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual BundleOptionGroups Group { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants Variant { get; set; } = null!;
}
