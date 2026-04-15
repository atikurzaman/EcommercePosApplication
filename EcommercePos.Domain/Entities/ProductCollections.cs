using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ProductCollections : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public bool ShowInHomePage { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<ProductCollectionItems> ProductCollectionItems { get; set; } = new List<ProductCollectionItems>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
