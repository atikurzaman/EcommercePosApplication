using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class CartItems
{
    public Guid Id { get; set; }

    public Guid CartId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime AddedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Carts Cart { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
