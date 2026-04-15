using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class CashDrawerEvents
{
    public Guid Id { get; set; }

    public Guid CashShiftId { get; set; }

    public Guid PerformedBy { get; set; }

    public Guid? TransactionId { get; set; }

    public string EventType { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? Notes { get; set; }

    public DateTime OccurredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual CashShifts CashShift { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users PerformedByNavigation { get; set; } = null!;

    public virtual PosTransactions? Transaction { get; set; }
}
