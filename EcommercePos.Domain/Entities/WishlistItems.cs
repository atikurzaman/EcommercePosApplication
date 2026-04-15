using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class WishlistItems : AuditableEntity<Guid>
{
    public Guid WishlistId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public string? Notes { get; set; }

    public int Priority { get; set; }

    public DateTime AddedAt { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }

    public virtual Wishlists Wishlist { get; set; } = null!;
}
