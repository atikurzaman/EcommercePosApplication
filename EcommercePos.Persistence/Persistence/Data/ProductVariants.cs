using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class ProductVariants
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Sku { get; set; }

    public string? Barcode { get; set; }

    public decimal CostPrice { get; set; }

    public decimal PriceModifier { get; set; }

    public decimal? OverridePrice { get; set; }

    public decimal? WeightKg { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<BundleComponents> BundleComponents { get; set; } = new List<BundleComponents>();

    public virtual ICollection<BundleOptionItems> BundleOptionItems { get; set; } = new List<BundleOptionItems>();

    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<GoodsReceiptLines> GoodsReceiptLines { get; set; } = new List<GoodsReceiptLines>();

    public virtual ICollection<InventoryAdjustmentLines> InventoryAdjustmentLines { get; set; } = new List<InventoryAdjustmentLines>();

    public virtual ICollection<OrderBundleSelections> OrderBundleSelections { get; set; } = new List<OrderBundleSelections>();

    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

    public virtual ICollection<PosTransactionBundleSelections> PosTransactionBundleSelections { get; set; } = new List<PosTransactionBundleSelections>();

    public virtual ICollection<PosTransactionLines> PosTransactionLines { get; set; } = new List<PosTransactionLines>();

    public virtual ICollection<PosTransactionReturnLines> PosTransactionReturnLines { get; set; } = new List<PosTransactionReturnLines>();

    public virtual ICollection<PriceListItems> PriceListItems { get; set; } = new List<PriceListItems>();

    public virtual Products Product { get; set; } = null!;

    public virtual ICollection<ProductImages> ProductImages { get; set; } = new List<ProductImages>();

    public virtual ICollection<ProductMedia> ProductMedia { get; set; } = new List<ProductMedia>();

    public virtual ICollection<ProductSpecificationValues> ProductSpecificationValues { get; set; } = new List<ProductSpecificationValues>();

    public virtual ICollection<PurchaseOrderLines> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLines>();

    public virtual ICollection<PurchaseReturnLines> PurchaseReturnLines { get; set; } = new List<PurchaseReturnLines>();

    public virtual ICollection<QuoteItems> QuoteItems { get; set; } = new List<QuoteItems>();

    public virtual ICollection<ReorderRules> ReorderRules { get; set; } = new List<ReorderRules>();

    public virtual ICollection<StockItems> StockItems { get; set; } = new List<StockItems>();

    public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();

    public virtual ICollection<StockTransferLines> StockTransferLines { get; set; } = new List<StockTransferLines>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<VariantAttributeMatrix> VariantAttributeMatrix { get; set; } = new List<VariantAttributeMatrix>();

    public virtual ICollection<VariantAttributeOptions> VariantAttributeOptions { get; set; } = new List<VariantAttributeOptions>();

    public virtual ICollection<WishlistItems> WishlistItems { get; set; } = new List<WishlistItems>();
}
