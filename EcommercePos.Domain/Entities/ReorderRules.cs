using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ReorderRules
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? WarehouseId { get; set; }

    public Guid? PreferredSupplierId { get; set; }

    public decimal ReorderLevel { get; set; }

    public decimal ReorderQuantity { get; set; }

    public Guid? NotifyUserId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? NotifyUser { get; set; }

    public virtual Suppliers? PreferredSupplier { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }

    public virtual Warehouses? Warehouse { get; set; }
}
