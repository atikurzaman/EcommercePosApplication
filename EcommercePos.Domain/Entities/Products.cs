using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Products
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? BrandId { get; set; }

    public Guid? ColorId { get; set; }

    public Guid? UnitId { get; set; }

    public Guid? TaxRateId { get; set; }

    public string? ConditionCode { get; set; }

    public Guid? SellerId { get; set; }

    public string? ProductCode { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? Sku { get; set; }

    public string? Barcode { get; set; }

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public string ProductType { get; set; } = null!;

    public decimal CostPrice { get; set; }

    public decimal SalePrice { get; set; }

    public decimal? OriginalPrice { get; set; }

    public bool IsTaxInclusive { get; set; }

    public decimal? WeightKg { get; set; }

    public string? Dimensions { get; set; }

    public string? ShelfLocation { get; set; }

    public int MinimumStockLevel { get; set; }

    public decimal ReorderLevel { get; set; }

    public decimal MinSaleQty { get; set; }

    public decimal? MaxSaleQty { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsBestSeller { get; set; }

    public bool IsNewArrival { get; set; }

    public bool IsPerishable { get; set; }

    public bool HasExpiry { get; set; }

    public bool IsActive { get; set; }

    public int ViewCount { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<AttributeOptionMedia> AttributeOptionMedia { get; set; } = new List<AttributeOptionMedia>();

    public virtual Brands? Brand { get; set; }

    public virtual ICollection<BundleComponents> BundleComponents { get; set; } = new List<BundleComponents>();

    public virtual ICollection<BundleOptionGroups> BundleOptionGroups { get; set; } = new List<BundleOptionGroups>();

    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();

    public virtual Categories Category { get; set; } = null!;

    public virtual Colors? Color { get; set; }

    public virtual ProductConditions? ConditionCodeNavigation { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<DiscountApplicability> DiscountApplicability { get; set; } = new List<DiscountApplicability>();

    public virtual ICollection<FlashDealProducts> FlashDealProducts { get; set; } = new List<FlashDealProducts>();

    public virtual ICollection<GoodsReceiptLines> GoodsReceiptLines { get; set; } = new List<GoodsReceiptLines>();

    public virtual ICollection<InventoryAdjustmentLines> InventoryAdjustmentLines { get; set; } = new List<InventoryAdjustmentLines>();

    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

    public virtual ICollection<PosTransactionLines> PosTransactionLines { get; set; } = new List<PosTransactionLines>();

    public virtual ICollection<PosTransactionReturnLines> PosTransactionReturnLines { get; set; } = new List<PosTransactionReturnLines>();

    public virtual ICollection<PriceListItems> PriceListItems { get; set; } = new List<PriceListItems>();

    public virtual ICollection<ProductAttributeLinks> ProductAttributeLinks { get; set; } = new List<ProductAttributeLinks>();

    public virtual ICollection<ProductBatches> ProductBatches { get; set; } = new List<ProductBatches>();

    public virtual ICollection<ProductCollectionItems> ProductCollectionItems { get; set; } = new List<ProductCollectionItems>();

    public virtual ICollection<ProductImages> ProductImages { get; set; } = new List<ProductImages>();

    public virtual ICollection<ProductMedia> ProductMedia { get; set; } = new List<ProductMedia>();

    public virtual ICollection<ProductPriceHistories> ProductPriceHistories { get; set; } = new List<ProductPriceHistories>();

    public virtual ICollection<ProductSpecificationValues> ProductSpecificationValues { get; set; } = new List<ProductSpecificationValues>();

    public virtual ICollection<ProductSupplierLinks> ProductSupplierLinks { get; set; } = new List<ProductSupplierLinks>();

    public virtual ICollection<ProductTags> ProductTags { get; set; } = new List<ProductTags>();

    public virtual ICollection<ProductTaxRates> ProductTaxRates { get; set; } = new List<ProductTaxRates>();

    public virtual ICollection<ProductVariants> ProductVariants { get; set; } = new List<ProductVariants>();

    public virtual ICollection<PurchaseOrderLines> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLines>();

    public virtual ICollection<PurchaseReturnLines> PurchaseReturnLines { get; set; } = new List<PurchaseReturnLines>();

    public virtual ICollection<QuoteItems> QuoteItems { get; set; } = new List<QuoteItems>();

    public virtual ICollection<ReorderRules> ReorderRules { get; set; } = new List<ReorderRules>();

    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();

    public virtual Sellers? Seller { get; set; }

    public virtual ICollection<StockItems> StockItems { get; set; } = new List<StockItems>();

    public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();

    public virtual ICollection<StockTransferLines> StockTransferLines { get; set; } = new List<StockTransferLines>();

    public virtual TaxRates? TaxRate { get; set; }

    public virtual Units? Unit { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<VariantAttributeMatrix> VariantAttributeMatrix { get; set; } = new List<VariantAttributeMatrix>();

    public virtual ICollection<WishlistItems> WishlistItems { get; set; } = new List<WishlistItems>();
}
