using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Tags : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<ProductTags> ProductTags { get; set; } = new List<ProductTags>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
