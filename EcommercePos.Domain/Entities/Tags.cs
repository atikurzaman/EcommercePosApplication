using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Tags
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<ProductTags> ProductTags { get; set; } = new List<ProductTags>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
