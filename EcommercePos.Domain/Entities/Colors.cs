using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Colors
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? HexCode { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<AttributeOptions> AttributeOptions { get; set; } = new List<AttributeOptions>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
