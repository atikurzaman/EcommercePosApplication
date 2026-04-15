using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class StockTransfers : AuditableEntity<Guid>
{
    public string TransferNo { get; set; } = null!;

    public Guid FromWarehouseId { get; set; }

    public Guid ToWarehouseId { get; set; }

    public DateTime TransferDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? CreatedByUser { get; set; }

    public virtual Warehouses FromWarehouse { get; set; } = null!;

    public virtual ICollection<StockTransferLines> StockTransferLines { get; set; } = new List<StockTransferLines>();

    public virtual Warehouses ToWarehouse { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
