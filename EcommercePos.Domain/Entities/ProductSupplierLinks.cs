using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ProductSupplierLinks
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid SupplierId { get; set; }

    public string? SupplierSku { get; set; }

    public decimal? UnitCost { get; set; }

    public int? LeadTimeDays { get; set; }

    public bool IsPreferred { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual Suppliers Supplier { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
