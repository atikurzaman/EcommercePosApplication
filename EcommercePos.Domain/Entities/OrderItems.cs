using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class OrderItems : AuditableEntity<Guid>
{
    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid? SellerId { get; set; }

    public Guid? BatchId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? VariantName { get; set; }

    public string? Sku { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal UnitCost { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal TotalCost { get; set; }

    public string? Notes { get; set; }
    public virtual ProductBatches? Batch { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Orders Order { get; set; } = null!;

    public virtual ICollection<OrderBundleSelections> OrderBundleSelections { get; set; } = new List<OrderBundleSelections>();

    public virtual ICollection<OrderItemTaxes> OrderItemTaxes { get; set; } = new List<OrderItemTaxes>();

    public virtual ICollection<OrderReturnItems> OrderReturnItems { get; set; } = new List<OrderReturnItems>();

    public virtual Products Product { get; set; } = null!;

    public virtual Sellers? Seller { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
