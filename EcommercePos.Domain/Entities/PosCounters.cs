using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class PosCounters : AuditableEntity<Guid>
{
    public Guid WarehouseId { get; set; }

    public string CounterCode { get; set; } = null!;

    public string CounterName { get; set; } = null!;

    public bool IsActive { get; set; }
    public virtual ICollection<CashShifts> CashShifts { get; set; } = new List<CashShifts>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<PosTerminals> PosTerminals { get; set; } = new List<PosTerminals>();

    public virtual ICollection<PosTransactions> PosTransactions { get; set; } = new List<PosTransactions>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
