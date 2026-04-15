using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class VwStockAvailability
{
    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? BatchId { get; set; }

    public Guid WarehouseId { get; set; }

    public string WarehouseName { get; set; } = null!;

    public string SiteType { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? VariantName { get; set; }

    public decimal QuantityOnHand { get; set; }

    public decimal ReservedQuantity { get; set; }

    public decimal? AvailableQty { get; set; }

    public decimal? ReorderLevel { get; set; }

    public decimal AverageCostPrice { get; set; }

    public int NeedsReorder { get; set; }
}
