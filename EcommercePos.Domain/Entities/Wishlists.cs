using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Wishlists : AuditableEntity<Guid>
{
    public Guid? CustomerId { get; set; }

    public Guid? UserId { get; set; }

    public string WishlistTypeCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? SharingToken { get; set; }

    public bool IsPublic { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers? Customer { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }

    public virtual ICollection<WishlistItems> WishlistItems { get; set; } = new List<WishlistItems>();

    public virtual WishlistTypes WishlistTypeCodeNavigation { get; set; } = null!;
}
