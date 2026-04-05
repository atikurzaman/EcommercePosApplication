using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class ProductCollections
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public bool ShowInHomePage { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<ProductCollectionItems> ProductCollectionItems { get; set; } = new List<ProductCollectionItems>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
