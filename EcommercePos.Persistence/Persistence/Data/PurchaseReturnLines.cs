using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class PurchaseReturnLines
{
    public Guid Id { get; set; }

    public Guid PurchaseReturnId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal LineTotal { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual PurchaseReturns PurchaseReturn { get; set; } = null!;

    public virtual ProductVariants? Variant { get; set; }
}
