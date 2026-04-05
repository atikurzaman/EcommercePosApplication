using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class StockItems
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public Guid WarehouseId { get; set; }

    public decimal QuantityOnHand { get; set; }

    public decimal ReservedQuantity { get; set; }

    public decimal AverageCostPrice { get; set; }

    public decimal? ReorderLevel { get; set; }

    public DateTime? LastCountDate { get; set; }

    public Guid? CountedByUserId { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CountedByUser { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
