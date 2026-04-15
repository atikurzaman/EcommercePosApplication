using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class LoyaltyTransactions : AuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? PosTransId { get; set; }

    public string TransactionType { get; set; } = null!;

    public int Points { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsUsed { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual Orders? Order { get; set; }

    public virtual PosTransactions? PosTrans { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
