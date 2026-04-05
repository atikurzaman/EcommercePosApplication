using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class WalletTransactions
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceAfter { get; set; }

    public string TransactionType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Reference { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual CustomerWallets Wallet { get; set; } = null!;
}
