using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class LoyaltyTransactions
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? PosTransId { get; set; }

    public string TransactionType { get; set; } = null!;

    public int Points { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual Orders? Order { get; set; }

    public virtual PosTransactions? PosTrans { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
