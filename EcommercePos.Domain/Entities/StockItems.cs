using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class StockItems : AuditableEntity<Guid>
{
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
    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CountedByUser { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
