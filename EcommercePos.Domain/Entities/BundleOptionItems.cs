using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class BundleOptionItems : AuditableEntity<Guid>
{
    public Guid GroupId { get; set; }

    public Guid VariantId { get; set; }

    public decimal PriceAdjustment { get; set; }

    public bool IsDefault { get; set; }

    public int SortOrder { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual BundleOptionGroups Group { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants Variant { get; set; } = null!;
}
