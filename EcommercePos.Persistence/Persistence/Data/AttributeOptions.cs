using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class AttributeOptions
{
    public Guid Id { get; set; }

    public Guid AttributeTypeId { get; set; }

    public Guid? ColorId { get; set; }

    public string Value { get; set; } = null!;

    public string? DisplayValue { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<AttributeOptionMedia> AttributeOptionMedia { get; set; } = new List<AttributeOptionMedia>();

    public virtual AttributeTypes AttributeType { get; set; } = null!;

    public virtual Colors? Color { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<VariantAttributeOptions> VariantAttributeOptions { get; set; } = new List<VariantAttributeOptions>();
}
