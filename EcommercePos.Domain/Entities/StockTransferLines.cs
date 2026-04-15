using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class StockTransferLines
{
    public Guid Id { get; set; }

    public Guid TransferId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual StockTransfers Transfer { get; set; } = null!;

    public virtual ProductVariants? Variant { get; set; }
}
