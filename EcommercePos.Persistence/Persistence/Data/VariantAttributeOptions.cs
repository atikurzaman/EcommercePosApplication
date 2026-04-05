using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class VariantAttributeOptions
{
    public Guid Id { get; set; }

    public Guid VariantId { get; set; }

    public Guid OptionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual AttributeOptions Option { get; set; } = null!;

    public virtual ProductVariants Variant { get; set; } = null!;
}
