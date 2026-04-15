using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ProductTaxRates : AuditableEntity<Guid>
{
    public Guid ProductId { get; set; }

    public Guid TaxRateId { get; set; }

    public bool IsActive { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual TaxRates TaxRate { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
