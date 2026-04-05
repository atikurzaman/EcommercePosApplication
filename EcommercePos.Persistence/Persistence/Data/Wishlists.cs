using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class Wishlists
{
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? UserId { get; set; }

    public string WishlistTypeCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? SharingToken { get; set; }

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers? Customer { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }

    public virtual ICollection<WishlistItems> WishlistItems { get; set; } = new List<WishlistItems>();

    public virtual WishlistTypes WishlistTypeCodeNavigation { get; set; } = null!;
}
