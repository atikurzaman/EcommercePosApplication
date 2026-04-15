using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class BundleComponents : AuditableEntity<Guid>
{
    public Guid BundleProductId { get; set; }

    public Guid ComponentVariantId { get; set; }

    public decimal Quantity { get; set; }

    public bool IsSubstitutable { get; set; }

    public int SortOrder { get; set; }
    public virtual Products BundleProduct { get; set; } = null!;

    public virtual ProductVariants ComponentVariant { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
