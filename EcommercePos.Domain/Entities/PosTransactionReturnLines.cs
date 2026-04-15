using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PosTransactionReturnLines
{
    public Guid Id { get; set; }

    public Guid PosTransactionReturnId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PosTransactionReturns PosTransactionReturn { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual ProductVariants? Variant { get; set; }
}
