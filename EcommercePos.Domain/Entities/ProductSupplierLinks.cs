using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ProductSupplierLinks : AuditableEntity<Guid>
{
    public Guid ProductId { get; set; }

    public Guid SupplierId { get; set; }

    public string? SupplierSku { get; set; }

    public decimal? UnitCost { get; set; }

    public int? LeadTimeDays { get; set; }

    public bool IsPreferred { get; set; }

    public bool IsActive { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual Suppliers Supplier { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
