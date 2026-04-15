using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ProductTaxRates
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid TaxRateId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual TaxRates TaxRate { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
