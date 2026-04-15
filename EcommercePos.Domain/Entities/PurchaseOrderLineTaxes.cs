using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PurchaseOrderLineTaxes
{
    public Guid Id { get; set; }

    public Guid PurchaseOrderLineId { get; set; }

    public Guid TaxRateId { get; set; }

    public string TaxName { get; set; } = null!;

    public decimal TaxRate { get; set; }

    public decimal TaxAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PurchaseOrderLines PurchaseOrderLine { get; set; } = null!;

    public virtual TaxRates TaxRateNavigation { get; set; } = null!;
}
