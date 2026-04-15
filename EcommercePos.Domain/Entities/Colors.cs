using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Colors : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string? HexCode { get; set; }

    public bool IsActive { get; set; }
    public virtual ICollection<AttributeOptions> AttributeOptions { get; set; } = new List<AttributeOptions>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
