using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class WishlistItems
{
    public Guid Id { get; set; }

    public Guid WishlistId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public string? Notes { get; set; }

    public int Priority { get; set; }

    public DateTime AddedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }

    public virtual Wishlists Wishlist { get; set; } = null!;
}
