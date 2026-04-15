using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PosCounters
{
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    public string CounterCode { get; set; } = null!;

    public string CounterName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<CashShifts> CashShifts { get; set; } = new List<CashShifts>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<PosTerminals> PosTerminals { get; set; } = new List<PosTerminals>();

    public virtual ICollection<PosTransactions> PosTransactions { get; set; } = new List<PosTransactions>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
