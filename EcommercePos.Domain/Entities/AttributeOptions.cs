using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class AttributeOptions : AuditableEntity<Guid>
{
    public Guid AttributeTypeId { get; set; }

    public Guid? ColorId { get; set; }

    public string Value { get; set; } = null!;

    public string? DisplayValue { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
    public virtual ICollection<AttributeOptionMedia> AttributeOptionMedia { get; set; } = new List<AttributeOptionMedia>();

    public virtual AttributeTypes AttributeType { get; set; } = null!;

    public virtual Colors? Color { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<VariantAttributeOptions> VariantAttributeOptions { get; set; } = new List<VariantAttributeOptions>();
}
