using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class StockTransfers
{
    public Guid Id { get; set; }

    public string TransferNo { get; set; } = null!;

    public Guid FromWarehouseId { get; set; }

    public Guid ToWarehouseId { get; set; }

    public DateTime TransferDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? CreatedByUser { get; set; }

    public virtual Warehouses FromWarehouse { get; set; } = null!;

    public virtual ICollection<StockTransferLines> StockTransferLines { get; set; } = new List<StockTransferLines>();

    public virtual Warehouses ToWarehouse { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
