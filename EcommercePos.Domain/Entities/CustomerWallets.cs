using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class CustomerWallets
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public decimal Balance { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Currencies CurrencyCodeNavigation { get; set; } = null!;

    public virtual Customers Customer { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<WalletTransactions> WalletTransactions { get; set; } = new List<WalletTransactions>();
}
