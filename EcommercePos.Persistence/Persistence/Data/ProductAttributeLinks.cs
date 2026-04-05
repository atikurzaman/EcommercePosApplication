using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class ProductAttributeLinks
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid AttributeTypeId { get; set; }

    public bool IsRequired { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual AttributeTypes AttributeType { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;
}
