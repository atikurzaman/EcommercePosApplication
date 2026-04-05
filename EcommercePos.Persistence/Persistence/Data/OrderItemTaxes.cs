using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class OrderItemTaxes
{
    public Guid Id { get; set; }

    public Guid OrderItemId { get; set; }

    public Guid TaxRateId { get; set; }

    public string TaxName { get; set; } = null!;

    public decimal TaxRate { get; set; }

    public decimal TaxAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual OrderItems OrderItem { get; set; } = null!;

    public virtual TaxRates TaxRateNavigation { get; set; } = null!;
}
