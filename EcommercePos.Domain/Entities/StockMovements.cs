using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class StockMovements
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public Guid? StockItemId { get; set; }

    public Guid? FromWarehouseId { get; set; }

    public Guid? ToWarehouseId { get; set; }

    public string MovementTypeCode { get; set; } = null!;

    public decimal QuantityIn { get; set; }

    public decimal QuantityOut { get; set; }

    public decimal BalanceAfter { get; set; }

    public decimal? UnitCost { get; set; }

    public string? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? Notes { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Warehouses? FromWarehouse { get; set; }

    public virtual StockMovementTypes MovementTypeCodeNavigation { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual StockItems? StockItem { get; set; }

    public virtual Warehouses? ToWarehouse { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
