using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class AttributeTypes
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string UiType { get; set; } = null!;

    public bool AffectsPrice { get; set; }

    public bool AffectsSku { get; set; }

    public bool AffectsImage { get; set; }

    public bool AffectsStock { get; set; }

    public bool IsFilterable { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<AttributeOptions> AttributeOptions { get; set; } = new List<AttributeOptions>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<ProductAttributeLinks> ProductAttributeLinks { get; set; } = new List<ProductAttributeLinks>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
