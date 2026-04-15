using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class VariantAttributeMatrix
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid VariantId { get; set; }

    public string AttributeCombination { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ProductVariants Variant { get; set; } = null!;
}
