using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PosTerminals
{
    public Guid Id { get; set; }

    public Guid PosCounterId { get; set; }

    public string TerminalCode { get; set; } = null!;

    public string TerminalName { get; set; } = null!;

    public string? MachineName { get; set; }

    public string? Ipaddress { get; set; }

    public string? PrinterName { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<CashShifts> CashShifts { get; set; } = new List<CashShifts>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PosCounters PosCounter { get; set; } = null!;

    public virtual ICollection<PosTransactions> PosTransactions { get; set; } = new List<PosTransactions>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
