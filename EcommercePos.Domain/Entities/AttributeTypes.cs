using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class AttributeTypes : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string UiType { get; set; } = null!;

    public bool AffectsPrice { get; set; }

    public bool AffectsSku { get; set; }

    public bool AffectsImage { get; set; }

    public bool AffectsStock { get; set; }

    public bool IsFilterable { get; set; }

    public int SortOrder { get; set; }
    public virtual ICollection<AttributeOptions> AttributeOptions { get; set; } = new List<AttributeOptions>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<ProductAttributeLinks> ProductAttributeLinks { get; set; } = new List<ProductAttributeLinks>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
