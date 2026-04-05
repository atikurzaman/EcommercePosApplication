using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class ProductCollectionItems
{
    public Guid Id { get; set; }

    public Guid ProductCollectionId { get; set; }

    public Guid ProductId { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ProductCollections ProductCollection { get; set; } = null!;
}
