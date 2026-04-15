using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class InventoryAdjustments : AuditableEntity<Guid>
{
    public string AdjustmentNo { get; set; } = null!;

    public Guid WarehouseId { get; set; }

    public DateTime AdjustmentDate { get; set; }

    public string AdjustmentType { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public string? Notes { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public virtual Users? ApprovedByUser { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<InventoryAdjustmentLines> InventoryAdjustmentLines { get; set; } = new List<InventoryAdjustmentLines>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
