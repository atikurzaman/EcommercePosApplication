using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class CashShifts
{
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid PosCounterId { get; set; }

    public Guid? PosTerminalId { get; set; }

    public Guid? OpenedByEmployeeId { get; set; }

    public Guid? ClosedByEmployeeId { get; set; }

    public Guid? OpenedByUserId { get; set; }

    public Guid? ClosedByUserId { get; set; }

    public DateTime OpeningDateTime { get; set; }

    public DateTime? ClosingDateTime { get; set; }

    public decimal OpeningCash { get; set; }

    public decimal? ClosingCash { get; set; }

    public decimal? ExpectedCash { get; set; }

    public decimal? CashVariance { get; set; }

    public decimal TotalSalesAmount { get; set; }

    public int TotalTransactions { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<CashDrawerEvents> CashDrawerEvents { get; set; } = new List<CashDrawerEvents>();

    public virtual Employees? ClosedByEmployee { get; set; }

    public virtual Users? ClosedByUser { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<DayEndSummaries> DayEndSummaries { get; set; } = new List<DayEndSummaries>();

    public virtual Employees? OpenedByEmployee { get; set; }

    public virtual Users? OpenedByUser { get; set; }

    public virtual PosCounters PosCounter { get; set; } = null!;

    public virtual PosTerminals? PosTerminal { get; set; }

    public virtual ICollection<PosTransactions> PosTransactions { get; set; } = new List<PosTransactions>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
