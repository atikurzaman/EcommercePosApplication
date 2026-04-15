using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Currencies
{
    public string CurrencyCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public byte DecimalPlaces { get; set; }

    public bool IsBaseCurrency { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<CustomerWallets> CustomerWallets { get; set; } = new List<CustomerWallets>();

    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
