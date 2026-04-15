using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class InventoryAdjustments
{
    public Guid Id { get; set; }

    public string AdjustmentNo { get; set; } = null!;

    public Guid WarehouseId { get; set; }

    public DateTime AdjustmentDate { get; set; }

    public string AdjustmentType { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public string? Notes { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? ApprovedByUser { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<InventoryAdjustmentLines> InventoryAdjustmentLines { get; set; } = new List<InventoryAdjustmentLines>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
