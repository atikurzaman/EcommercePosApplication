using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class CustomerWallets : AuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal Balance { get; set; }

    public bool IsActive { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Currencies CurrencyCodeNavigation { get; set; } = null!;

    public virtual Customers Customer { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<WalletTransactions> WalletTransactions { get; set; } = new List<WalletTransactions>();
}
