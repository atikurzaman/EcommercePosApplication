using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class TaxRates : AuditableEntity<Guid>
{
    public string TaxCode { get; set; } = null!;

    public string TaxName { get; set; } = null!;

    public string TaxType { get; set; } = null!;

    public decimal Rate { get; set; }

    public bool IsPercentage { get; set; }

    public bool IsInclusive { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public string Country { get; set; } = null!;

    public bool ApplyToShipping { get; set; }

    public int Priority { get; set; }

    public string? Description { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<OrderItemTaxes> OrderItemTaxes { get; set; } = new List<OrderItemTaxes>();

    public virtual ICollection<PosTransactionLineTaxes> PosTransactionLineTaxes { get; set; } = new List<PosTransactionLineTaxes>();

    public virtual ICollection<ProductTaxRates> ProductTaxRates { get; set; } = new List<ProductTaxRates>();

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual ICollection<PurchaseOrderLineTaxes> PurchaseOrderLineTaxes { get; set; } = new List<PurchaseOrderLineTaxes>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
