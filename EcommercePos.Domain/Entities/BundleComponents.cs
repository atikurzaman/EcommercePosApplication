using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class BundleComponents
{
    public Guid Id { get; set; }

    public Guid BundleProductId { get; set; }

    public Guid ComponentVariantId { get; set; }

    public decimal Quantity { get; set; }

    public bool IsSubstitutable { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Products BundleProduct { get; set; } = null!;

    public virtual ProductVariants ComponentVariant { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
