using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class WalletTransactions : AuditableEntity<Guid>
{
    public Guid WalletId { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceAfter { get; set; }

    public string TransactionType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Reference { get; set; }

    public string? Description { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual CustomerWallets Wallet { get; set; } = null!;
}
