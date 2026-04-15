using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PosTransactionLines
{
    public Guid Id { get; set; }

    public Guid TransactionId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Sku { get; set; }

    public string? Barcode { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal LineTotal { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<PosTransactionBundleSelections> PosTransactionBundleSelections { get; set; } = new List<PosTransactionBundleSelections>();

    public virtual ICollection<PosTransactionLineTaxes> PosTransactionLineTaxes { get; set; } = new List<PosTransactionLineTaxes>();

    public virtual Products Product { get; set; } = null!;

    public virtual PosTransactions Transaction { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
