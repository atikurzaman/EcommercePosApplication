using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ReorderRules : AuditableEntity<Guid>
{
    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? WarehouseId { get; set; }

    public Guid? PreferredSupplierId { get; set; }

    public decimal ReorderLevel { get; set; }

    public decimal ReorderQuantity { get; set; }

    public Guid? NotifyUserId { get; set; }

    public bool IsActive { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? NotifyUser { get; set; }

    public virtual Suppliers? PreferredSupplier { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }

    public virtual Warehouses? Warehouse { get; set; }
}
