using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;

namespace EcommercePos.Persistence.Data;

public partial class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLogs> ActivityLogs { get; set; }

    public virtual DbSet<AppSettings> AppSettings { get; set; }

    public virtual DbSet<AttributeOptionMedia> AttributeOptionMedia { get; set; }

    public virtual DbSet<AttributeOptionMediaBlob> AttributeOptionMediaBlob { get; set; }

    public virtual DbSet<AttributeOptions> AttributeOptions { get; set; }

    public virtual DbSet<AttributeTypes> AttributeTypes { get; set; }

    public virtual DbSet<AuditLogs> AuditLogs { get; set; }

    public virtual DbSet<BlogCategories> BlogCategories { get; set; }

    public virtual DbSet<BlogComments> BlogComments { get; set; }

    public virtual DbSet<BlogTags> BlogTags { get; set; }

    public virtual DbSet<Blogs> Blogs { get; set; }

    public virtual DbSet<Brands> Brands { get; set; }

    public virtual DbSet<BundleComponents> BundleComponents { get; set; }

    public virtual DbSet<BundleOptionGroups> BundleOptionGroups { get; set; }

    public virtual DbSet<BundleOptionItems> BundleOptionItems { get; set; }

    public virtual DbSet<CartItems> CartItems { get; set; }

    public virtual DbSet<Carts> Carts { get; set; }

    public virtual DbSet<CashDrawerEvents> CashDrawerEvents { get; set; }

    public virtual DbSet<CashShifts> CashShifts { get; set; }

    public virtual DbSet<Categories> Categories { get; set; }

    public virtual DbSet<Colors> Colors { get; set; }

    public virtual DbSet<ContactMessages> ContactMessages { get; set; }

    public virtual DbSet<Currencies> Currencies { get; set; }

    public virtual DbSet<CustomerAddresses> CustomerAddresses { get; set; }

    public virtual DbSet<CustomerProfiles> CustomerProfiles { get; set; }

    public virtual DbSet<CustomerTiers> CustomerTiers { get; set; }

    public virtual DbSet<CustomerWallets> CustomerWallets { get; set; }

    public virtual DbSet<Customers> Customers { get; set; }

    public virtual DbSet<DayEndSummaries> DayEndSummaries { get; set; }

    public virtual DbSet<DeliveryZoneRegions> DeliveryZoneRegions { get; set; }

    public virtual DbSet<DeliveryZones> DeliveryZones { get; set; }

    public virtual DbSet<DiscountApplicability> DiscountApplicability { get; set; }

    public virtual DbSet<DiscountTypes> DiscountTypes { get; set; }

    public virtual DbSet<DiscountUsageLog> DiscountUsageLog { get; set; }

    public virtual DbSet<Discounts> Discounts { get; set; }

    public virtual DbSet<EmailTemplates> EmailTemplates { get; set; }

    public virtual DbSet<Employees> Employees { get; set; }

    public virtual DbSet<ExpenseCategories> ExpenseCategories { get; set; }

    public virtual DbSet<Expenses> Expenses { get; set; }

    public virtual DbSet<FlashDealProducts> FlashDealProducts { get; set; }

    public virtual DbSet<FlashDeals> FlashDeals { get; set; }

    public virtual DbSet<GoodsReceiptLines> GoodsReceiptLines { get; set; }

    public virtual DbSet<GoodsReceipts> GoodsReceipts { get; set; }

    public virtual DbSet<InventoryAdjustmentLines> InventoryAdjustmentLines { get; set; }

    public virtual DbSet<InventoryAdjustments> InventoryAdjustments { get; set; }

    public virtual DbSet<Invoices> Invoices { get; set; }

    public virtual DbSet<LoyaltyTransactions> LoyaltyTransactions { get; set; }

    public virtual DbSet<MediaAssets> MediaAssets { get; set; }

    public virtual DbSet<Menus> Menus { get; set; }

    public virtual DbSet<NewsletterSubscribers> NewsletterSubscribers { get; set; }

    public virtual DbSet<Notifications> Notifications { get; set; }

    public virtual DbSet<OrderBundleSelections> OrderBundleSelections { get; set; }

    public virtual DbSet<OrderItemTaxes> OrderItemTaxes { get; set; }

    public virtual DbSet<OrderItems> OrderItems { get; set; }

    public virtual DbSet<OrderReturnItems> OrderReturnItems { get; set; }

    public virtual DbSet<OrderStatuses> OrderStatuses { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<PaymentGateways> PaymentGateways { get; set; }

    public virtual DbSet<PaymentMethods> PaymentMethods { get; set; }

    public virtual DbSet<PaymentStatuses> PaymentStatuses { get; set; }

    public virtual DbSet<Payments> Payments { get; set; }

    public virtual DbSet<Permissions> Permissions { get; set; }

    public virtual DbSet<PickupPoints> PickupPoints { get; set; }

    public virtual DbSet<PosCounters> PosCounters { get; set; }

    public virtual DbSet<PosPaymentTenders> PosPaymentTenders { get; set; }

    public virtual DbSet<PosTerminals> PosTerminals { get; set; }

    public virtual DbSet<PosTransactionBundleSelections> PosTransactionBundleSelections { get; set; }

    public virtual DbSet<PosTransactionLineTaxes> PosTransactionLineTaxes { get; set; }

    public virtual DbSet<PosTransactionLines> PosTransactionLines { get; set; }

    public virtual DbSet<PosTransactionReturnLines> PosTransactionReturnLines { get; set; }

    public virtual DbSet<PosTransactionReturns> PosTransactionReturns { get; set; }

    public virtual DbSet<PosTransactions> PosTransactions { get; set; }

    public virtual DbSet<PriceListItems> PriceListItems { get; set; }

    public virtual DbSet<PriceLists> PriceLists { get; set; }

    public virtual DbSet<ProductAttributeLinks> ProductAttributeLinks { get; set; }

    public virtual DbSet<ProductBatches> ProductBatches { get; set; }

    public virtual DbSet<ProductCollectionItems> ProductCollectionItems { get; set; }

    public virtual DbSet<ProductCollections> ProductCollections { get; set; }

    public virtual DbSet<ProductConditions> ProductConditions { get; set; }

    public virtual DbSet<ProductImages> ProductImages { get; set; }

    public virtual DbSet<ProductMedia> ProductMedia { get; set; }

    public virtual DbSet<ProductMediaBlob> ProductMediaBlob { get; set; }

    public virtual DbSet<ProductPriceHistories> ProductPriceHistories { get; set; }

    public virtual DbSet<ProductSpecificationValues> ProductSpecificationValues { get; set; }

    public virtual DbSet<ProductSpecifications> ProductSpecifications { get; set; }

    public virtual DbSet<ProductSupplierLinks> ProductSupplierLinks { get; set; }

    public virtual DbSet<ProductTags> ProductTags { get; set; }

    public virtual DbSet<ProductTaxRates> ProductTaxRates { get; set; }

    public virtual DbSet<ProductVariants> ProductVariants { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<PurchaseOrderLineTaxes> PurchaseOrderLineTaxes { get; set; }

    public virtual DbSet<PurchaseOrderLines> PurchaseOrderLines { get; set; }

    public virtual DbSet<PurchaseOrders> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseReturnLines> PurchaseReturnLines { get; set; }

    public virtual DbSet<PurchaseReturns> PurchaseReturns { get; set; }

    public virtual DbSet<QuoteItems> QuoteItems { get; set; }

    public virtual DbSet<Quotes> Quotes { get; set; }

    public virtual DbSet<RefundRequests> RefundRequests { get; set; }

    public virtual DbSet<ReorderRules> ReorderRules { get; set; }

    public virtual DbSet<ReturnStatuses> ReturnStatuses { get; set; }

    public virtual DbSet<Returns> Returns { get; set; }

    public virtual DbSet<ReviewHelpfulness> ReviewHelpfulness { get; set; }

    public virtual DbSet<Reviews> Reviews { get; set; }

    public virtual DbSet<RoleClaims> RoleClaims { get; set; }

    public virtual DbSet<RoleMenus> RoleMenus { get; set; }

    public virtual DbSet<RolePermissions> RolePermissions { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<SearchKeywords> SearchKeywords { get; set; }

    public virtual DbSet<Sellers> Sellers { get; set; }

    public virtual DbSet<ShipmentStatuses> ShipmentStatuses { get; set; }

    public virtual DbSet<Shipments> Shipments { get; set; }

    public virtual DbSet<ShippingCarriers> ShippingCarriers { get; set; }

    public virtual DbSet<ShippingMethods> ShippingMethods { get; set; }

    public virtual DbSet<StaticPages> StaticPages { get; set; }

    public virtual DbSet<StockItems> StockItems { get; set; }

    public virtual DbSet<StockMovementTypes> StockMovementTypes { get; set; }

    public virtual DbSet<StockMovements> StockMovements { get; set; }

    public virtual DbSet<StockTransferLines> StockTransferLines { get; set; }

    public virtual DbSet<StockTransfers> StockTransfers { get; set; }

    public virtual DbSet<Suppliers> Suppliers { get; set; }

    public virtual DbSet<SupportTicketMessages> SupportTicketMessages { get; set; }

    public virtual DbSet<SupportTickets> SupportTickets { get; set; }

    public virtual DbSet<Tags> Tags { get; set; }

    public virtual DbSet<TaxRates> TaxRates { get; set; }

    public virtual DbSet<Units> Units { get; set; }

    public virtual DbSet<UserClaims> UserClaims { get; set; }

    public virtual DbSet<UserLogins> UserLogins { get; set; }

    public virtual DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }

    public virtual DbSet<UserRoles> UserRoles { get; set; }

    public virtual DbSet<UserTokens> UserTokens { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<VariantAttributeMatrix> VariantAttributeMatrix { get; set; }

    public virtual DbSet<VariantAttributeOptions> VariantAttributeOptions { get; set; }

    public virtual DbSet<VwCustomerLoyaltyBalance> VwCustomerLoyaltyBalance { get; set; }

    public virtual DbSet<VwCustomerStats> VwCustomerStats { get; set; }

    public virtual DbSet<VwProductStats> VwProductStats { get; set; }

    public virtual DbSet<VwStockAvailability> VwStockAvailability { get; set; }

    public virtual DbSet<WalletTransactions> WalletTransactions { get; set; }

    public virtual DbSet<Warehouses> Warehouses { get; set; }

    public virtual DbSet<WishlistItems> WishlistItems { get; set; }

    public virtual DbSet<WishlistTypes> WishlistTypes { get; set; }

    public virtual DbSet<Wishlists> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<ActivityLogs>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.OccurredAt })
                .IsDescending(false, true)
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ActivityType).HasMaxLength(80);
            entity.Property(e => e.EntityType).HasMaxLength(60);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasIndex(e => e.Key).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Category).HasMaxLength(60);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DataType)
                .HasMaxLength(20)
                .HasDefaultValue("String");
            entity.Property(e => e.Key).HasMaxLength(120);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AppSettingsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AppSettingsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<AttributeOptionMedia>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AltText).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Etag)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("ETag");
            entity.Property(e => e.FileName).HasMaxLength(260);
            entity.Property(e => e.MimeType).HasMaxLength(80);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AttributeOptionMediaCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Option).WithMany(p => p.AttributeOptionMedia)
                .HasForeignKey(d => d.OptionId);

            entity.HasOne(d => d.Product).WithMany(p => p.AttributeOptionMedia)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AttributeOptionMediaUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<AttributeOptionMediaBlob>(entity =>
        {
            entity.HasKey(e => e.MediaId);

            entity.Property(e => e.MediaId).ValueGeneratedNever();

            entity.HasOne(d => d.Media).WithOne(p => p.AttributeOptionMediaBlob)
                .HasForeignKey<AttributeOptionMediaBlob>(d => d.MediaId);
        });

        modelBuilder.Entity<AttributeOptions>(entity =>
        {
            entity.HasIndex(e => new { e.AttributeTypeId, e.Value }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DisplayValue).HasMaxLength(120);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Value).HasMaxLength(120);

            entity.HasOne(d => d.AttributeType).WithMany(p => p.AttributeOptions)
                .HasForeignKey(d => d.AttributeTypeId);

            entity.HasOne(d => d.Color).WithMany(p => p.AttributeOptions)
                .HasForeignKey(d => d.ColorId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AttributeOptionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AttributeOptionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<AttributeTypes>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();

            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AffectsSku).HasDefaultValue(true);
            entity.Property(e => e.AffectsStock).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsFilterable).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(80);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(90);
            entity.Property(e => e.UiType)
                .HasMaxLength(20)
                .HasDefaultValue("Dropdown");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AttributeTypesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AttributeTypesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<AuditLogs>(entity =>
        {
            entity.HasIndex(e => e.OccurredAt).IsDescending();

            entity.HasIndex(e => new { e.EntityName, e.EntityId });

            entity.HasIndex(e => new { e.UserId, e.OccurredAt }).IsDescending(false, true);

            entity.Property(e => e.Action).HasMaxLength(120);
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.EntityId).HasMaxLength(60);
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<BlogCategories>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogCategoriesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BlogCategoriesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<BlogComments>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogComments)
                .HasForeignKey(d => d.BlogId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogCommentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BlogCommentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.BlogCommentsUser)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<BlogTags>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(80);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(80);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogTags)
                .HasForeignKey(d => d.CreatedBy);
        });

        modelBuilder.Entity<Blogs>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.MetaDescription).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(200);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(300);
            entity.Property(e => e.Title).HasMaxLength(300);

            entity.HasOne(d => d.Author).WithMany(p => p.BlogsAuthor)
                .HasForeignKey(d => d.AuthorId);

            entity.HasOne(d => d.Category).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.CategoryId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BlogsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasMany(d => d.BlogTag).WithMany(p => p.Blog)
                .UsingEntity<Dictionary<string, object>>(
                    "BlogPostTags",
                    r => r.HasOne<BlogTags>().WithMany()
                        .HasForeignKey("BlogTagId"),
                    l => l.HasOne<Blogs>().WithMany()
                        .HasForeignKey("BlogId"),
                    j =>
                    {
                        j.HasKey("BlogId", "BlogTagId");
                    });
        });

        modelBuilder.Entity<Brands>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CountryOfOrigin)
                .HasMaxLength(2)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(100);
            entity.Property(e => e.Website).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BrandsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BrandsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasMany(d => d.Category).WithMany(p => p.Brand)
                .UsingEntity<Dictionary<string, object>>(
                    "BrandCategories",
                    r => r.HasOne<Categories>().WithMany()
                        .HasForeignKey("CategoryId"),
                    l => l.HasOne<Brands>().WithMany()
                        .HasForeignKey("BrandId"),
                    j =>
                    {
                        j.HasKey("BrandId", "CategoryId");
                    });
        });

        modelBuilder.Entity<BundleComponents>(entity =>
        {
            entity.HasIndex(e => new { e.BundleProductId, e.ComponentVariantId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.BundleProduct).WithMany(p => p.BundleComponents)
                .HasForeignKey(d => d.BundleProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ComponentVariant).WithMany(p => p.BundleComponents)
                .HasForeignKey(d => d.ComponentVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BundleComponentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BundleComponentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<BundleOptionGroups>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.GroupName).HasMaxLength(100);
            entity.Property(e => e.IsRequired).HasDefaultValue(true);
            entity.Property(e => e.MaxSelections).HasDefaultValue(1);
            entity.Property(e => e.MinSelections).HasDefaultValue(1);
            entity.Property(e => e.QuantityPerSelection).HasDefaultValue(1);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.BundleProduct).WithMany(p => p.BundleOptionGroups)
                .HasForeignKey(d => d.BundleProductId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BundleOptionGroupsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BundleOptionGroupsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<BundleOptionItems>(entity =>
        {
            entity.HasIndex(e => new { e.GroupId, e.VariantId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PriceAdjustment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BundleOptionItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Group).WithMany(p => p.BundleOptionItems)
                .HasForeignKey(d => d.GroupId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BundleOptionItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.BundleOptionItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CartItems>(entity =>
        {
            entity.HasIndex(e => e.CartId).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.ProductId).HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Batch).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CartItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CartItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<Carts>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CouponCode).HasMaxLength(60);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SessionId).HasMaxLength(120);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AppliedDiscount).WithMany(p => p.Carts)
                .HasForeignKey(d => d.AppliedDiscountId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CartsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CartsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.CartsUser)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<CashDrawerEvents>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.EventType).HasMaxLength(25);
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CashShift).WithMany(p => p.CashDrawerEvents)
                .HasForeignKey(d => d.CashShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CashDrawerEventsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.CashDrawerEventsPerformedByNavigation)
                .HasForeignKey(d => d.PerformedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Transaction).WithMany(p => p.CashDrawerEvents)
                .HasForeignKey(d => d.TransactionId);
        });

        modelBuilder.Entity<CashShifts>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CashVariance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ClosingCash).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ExpectedCash).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OpeningCash).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OpeningDateTime).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");
            entity.Property(e => e.TotalSalesAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ClosedByEmployee).WithMany(p => p.CashShiftsClosedByEmployee)
                .HasForeignKey(d => d.ClosedByEmployeeId);

            entity.HasOne(d => d.ClosedByUser).WithMany(p => p.CashShiftsClosedByUser)
                .HasForeignKey(d => d.ClosedByUserId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CashShiftsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.OpenedByEmployee).WithMany(p => p.CashShiftsOpenedByEmployee)
                .HasForeignKey(d => d.OpenedByEmployeeId);

            entity.HasOne(d => d.OpenedByUser).WithMany(p => p.CashShiftsOpenedByUser)
                .HasForeignKey(d => d.OpenedByUserId);

            entity.HasOne(d => d.PosCounter).WithMany(p => p.CashShifts)
                .HasForeignKey(d => d.PosCounterId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PosTerminal).WithMany(p => p.CashShifts)
                .HasForeignKey(d => d.PosTerminalId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CashShiftsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.CashShifts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Categories>(entity =>
        {
            entity.HasIndex(e => e.ParentCategoryId).HasFilter("([ParentCategoryId] IS NOT NULL)");

            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MetaDescription).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CategoriesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CategoriesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<Colors>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.HexCode)
                .HasMaxLength(7)
                .IsFixedLength();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(80);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ColorsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ColorsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ContactMessages>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Subject).HasMaxLength(200);

            entity.HasOne(d => d.RepliedByNavigation).WithMany(p => p.ContactMessages)
                .HasForeignKey(d => d.RepliedBy);
        });

        modelBuilder.Entity<Currencies>(entity =>
        {
            entity.HasKey(e => e.CurrencyCode);

            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DecimalPlaces).HasDefaultValue((byte)2);
            entity.Property(e => e.ExchangeRate)
                .HasDefaultValue(1.0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Symbol).HasMaxLength(5);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CurrenciesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CurrenciesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<CustomerAddresses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Addresses");

            entity.HasIndex(e => e.CustomerId).HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddressLine1).HasMaxLength(500);
            entity.Property(e => e.AddressLine2).HasMaxLength(500);
            entity.Property(e => e.AddressType)
                .HasMaxLength(20)
                .HasDefaultValue("Shipping");
            entity.Property(e => e.AlternatePhone).HasMaxLength(20);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .HasDefaultValue("BD")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DeliveryInstructions).HasMaxLength(500);
            entity.Property(e => e.FullName).HasMaxLength(120);
            entity.Property(e => e.Label).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.State).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerAddressesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerAddresses)
                .HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerAddressesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.CustomerAddressesUser)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<CustomerProfiles>(entity =>
        {
            entity.HasIndex(e => e.CustomerId).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TierCode).HasMaxLength(20);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerProfilesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithOne(p => p.CustomerProfiles)
                .HasForeignKey<CustomerProfiles>(d => d.CustomerId);

            entity.HasOne(d => d.TierCodeNavigation).WithMany(p => p.CustomerProfiles)
                .HasForeignKey(d => d.TierCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerProfilesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<CustomerTiers>(entity =>
        {
            entity.HasKey(e => e.TierCode);

            entity.Property(e => e.TierCode).HasMaxLength(20);
            entity.Property(e => e.DiscountPct).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.DisplayName).HasMaxLength(80);
            entity.Property(e => e.MinLifetimeSpend).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PointsMultiplier)
                .HasDefaultValue(1.0m)
                .HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<CustomerWallets>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Wallets");

            entity.HasIndex(e => e.CustomerId).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValue("BDT")
                .IsFixedLength();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerWalletsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.CustomerWallets)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Customer).WithOne(p => p.CustomerWallets)
                .HasForeignKey<CustomerWallets>(d => d.CustomerId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerWalletsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<Customers>(entity =>
        {
            entity.HasIndex(e => e.CustomerCode).IsUnique();

            entity.HasIndex(e => e.ReferralCode)
                .IsUnique()
                .HasFilter("([ReferralCode] IS NOT NULL)");

            entity.HasIndex(e => e.UserId)
                .IsUnique()
                .HasFilter("([UserId] IS NOT NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.AlternatePhone).HasMaxLength(30);
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .HasDefaultValue("BD")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreditLimit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CustomerCode).HasMaxLength(50);
            entity.Property(e => e.CustomerGroup).HasMaxLength(50);
            entity.Property(e => e.CustomerType)
                .HasMaxLength(30)
                .HasDefaultValue("Retail");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.ReferralCode).HasMaxLength(20);
            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TaxNumber).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.ReferredByCustomer).WithMany(p => p.InverseReferredByCustomer)
                .HasForeignKey(d => d.ReferredByCustomerId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithOne(p => p.CustomersUser)
                .HasForeignKey<Customers>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<DayEndSummaries>(entity =>
        {
            entity.HasIndex(e => new { e.SummaryDate, e.WarehouseId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CashInHand).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CashOut).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ExpectedCash).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoyaltyPointsIssued).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoyaltyPointsRedeemed).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OpeningCash).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");
            entity.Property(e => e.TotalCardSales).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalCashSales).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalDiscount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalMobileSales).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalReturnAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalSalesAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTaxCollected).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Variance).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CashShift).WithMany(p => p.DayEndSummaries)
                .HasForeignKey(d => d.CashShiftId);

            entity.HasOne(d => d.ClosedByUser).WithMany(p => p.DayEndSummariesClosedByUser)
                .HasForeignKey(d => d.ClosedByUserId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DayEndSummariesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DayEndSummariesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.DayEndSummaries)
                .HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<DeliveryZoneRegions>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .HasDefaultValue("BD")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.State).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DeliveryZoneRegionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.DeliveryZone).WithMany(p => p.DeliveryZoneRegions)
                .HasForeignKey(d => d.DeliveryZoneId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DeliveryZoneRegionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<DeliveryZones>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BaseDeliveryCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.FreeDeliveryThreshold).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DeliveryZonesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DeliveryZonesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<DiscountApplicability>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Category).WithMany(p => p.DiscountApplicability)
                .HasForeignKey(d => d.CategoryId);

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountApplicability)
                .HasForeignKey(d => d.DiscountId);

            entity.HasOne(d => d.Product).WithMany(p => p.DiscountApplicability)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DiscountTypes>(entity =>
        {
            entity.HasKey(e => e.TypeCode);

            entity.Property(e => e.TypeCode).HasMaxLength(30);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
        });

        modelBuilder.Entity<DiscountUsageLog>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UsedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DiscountUsageLogCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.DiscountUsageLog)
                .HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountUsageLog)
                .HasForeignKey(d => d.DiscountId);

            entity.HasOne(d => d.Order).WithMany(p => p.DiscountUsageLog)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.PosTransaction).WithMany(p => p.DiscountUsageLog)
                .HasForeignKey(d => d.PosTransactionId);

            entity.HasOne(d => d.User).WithMany(p => p.DiscountUsageLogUser)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Discounts>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AppliesTo)
                .HasMaxLength(20)
                .HasDefaultValue("ALL");
            entity.Property(e => e.Code).HasMaxLength(60);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DiscountTypeCode).HasMaxLength(30);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaximumDiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinimumOrderAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TierCode).HasMaxLength(20);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DiscountsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.DiscountTypeCodeNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TierCodeNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.TierCode);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DiscountsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<EmailTemplates>(entity =>
        {
            entity.HasIndex(e => e.TemplateType).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Subject).HasMaxLength(300);
            entity.Property(e => e.TemplateType).HasMaxLength(60);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmailTemplatesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.EmailTemplatesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<Employees>(entity =>
        {
            entity.HasIndex(e => e.EmployeeCode).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.BankAccountNumber).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Designation).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(150);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(30);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeType).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.NationalId).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ShiftPattern).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmployeesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.EmployeesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.EmployeesUser)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Employees)
                .HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<ExpenseCategories>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ExpenseCategoriesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ExpenseCategoriesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<Expenses>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ExpenseDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.MethodCode).HasMaxLength(40);
            entity.Property(e => e.ReceiptReference).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ExpensesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ExpensesCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId);

            entity.HasOne(d => d.ExpenseCategory).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.ExpenseCategoryId);

            entity.HasOne(d => d.MethodCodeNavigation).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.MethodCode);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ExpensesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FlashDealProducts>(entity =>
        {
            entity.HasIndex(e => new { e.FlashDealId, e.ProductId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.DiscountType).HasDefaultValue((byte)1);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FlashDealProductsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.FlashDeal).WithMany(p => p.FlashDealProducts)
                .HasForeignKey(d => d.FlashDealId);

            entity.HasOne(d => d.Product).WithMany(p => p.FlashDealProducts)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.FlashDealProductsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<FlashDeals>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Deals");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FlashDealsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.FlashDealsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<GoodsReceiptLines>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Batch).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.GoodsReceipt).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.GoodsReceiptId);

            entity.HasOne(d => d.Product).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PurchaseOrderLine).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.PurchaseOrderLineId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Variant).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<GoodsReceipts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Receipts");

            entity.HasIndex(e => e.ReceiptNumber).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Condition).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReceiptNumber).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GoodsReceiptsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReceivedByUser).WithMany(p => p.GoodsReceiptsReceivedByUser)
                .HasForeignKey(d => d.ReceivedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.GoodsReceiptsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InventoryAdjustmentLines>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AdjustmentQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CountedQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Remarks).HasMaxLength(255);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SystemQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Batch).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.InventoryAdjustment).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.InventoryAdjustmentId);

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Variant).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<InventoryAdjustments>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Adjustments");

            entity.HasIndex(e => e.AdjustmentNo).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AdjustmentDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.AdjustmentNo).HasMaxLength(50);
            entity.Property(e => e.AdjustmentType).HasMaxLength(30);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.InventoryAdjustmentsApprovedByUser)
                .HasForeignKey(d => d.ApprovedByUserId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InventoryAdjustmentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InventoryAdjustmentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryAdjustments)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Invoices>(entity =>
        {
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AmountDue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.ShippingAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvoicesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InvoicesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<LoyaltyTransactions>(entity =>
        {
            entity.HasIndex(e => new { e.CustomerId, e.TransactionDate }).IsDescending(false, true);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.TransactionType).HasMaxLength(20);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.LoyaltyTransactionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.LoyaltyTransactions)
                .HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.Order).WithMany(p => p.LoyaltyTransactions)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.PosTrans).WithMany(p => p.LoyaltyTransactions)
                .HasForeignKey(d => d.PosTransId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.LoyaltyTransactionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<MediaAssets>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AltText).HasMaxLength(200);
            entity.Property(e => e.ContentType).HasMaxLength(80);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.FileName).HasMaxLength(260);
            entity.Property(e => e.OriginalName).HasMaxLength(260);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.StorageProvider)
                .HasMaxLength(20)
                .HasDefaultValue("Local");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MediaAssetsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.MediaAssetsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.MediaAssetsUploadedByNavigation)
                .HasForeignKey(d => d.UploadedBy);
        });

        modelBuilder.Entity<Menus>(entity =>
        {
            entity.HasIndex(e => e.MenuCode).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DisplayName).HasMaxLength(150);
            entity.Property(e => e.IconClass).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsVisible).HasDefaultValue(true);
            entity.Property(e => e.MenuCode).HasMaxLength(50);
            entity.Property(e => e.MenuLevel).HasDefaultValue((byte)1);
            entity.Property(e => e.MenuName).HasMaxLength(100);
            entity.Property(e => e.MenuUrl).HasMaxLength(300);
            entity.Property(e => e.PermissionCode).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MenusCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.ParentMenu).WithMany(p => p.InverseParentMenu)
                .HasForeignKey(d => d.ParentMenuId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.MenusUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<NewsletterSubscribers>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SubscribedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.NewsletterSubscribers)
                .HasForeignKey(d => d.CustomerId);
        });

        modelBuilder.Entity<Notifications>(entity =>
        {
            entity.HasIndex(e => e.UserId).HasFilter("([IsDeleted]=(0) AND [IsRead]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TargetRole).HasMaxLength(256);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.NotificationsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.NotificationsUser)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderBundleSelections>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PriceAdjustment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderBundleSelections)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Group).WithMany(p => p.OrderBundleSelections)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderBundleSelections)
                .HasForeignKey(d => d.OrderItemId);

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderBundleSelections)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrderItemTaxes>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxName).HasMaxLength(100);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(9, 4)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderItemTaxes)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderItemTaxes)
                .HasForeignKey(d => d.OrderItemId);

            entity.HasOne(d => d.TaxRateNavigation).WithMany(p => p.OrderItemTaxes)
                .HasForeignKey(d => d.TaxRateId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrderItems>(entity =>
        {
            entity.HasIndex(e => e.OrderId).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.ProductId).HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VariantName).HasMaxLength(120);

            entity.HasOne(d => d.Batch).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Seller).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.SellerId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<OrderReturnItems>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Condition)
                .HasMaxLength(50)
                .HasDefaultValue("Unknown");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderReturnItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderReturnItems)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Return).WithMany(p => p.OrderReturnItems)
                .HasForeignKey(d => d.ReturnId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderReturnItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<OrderStatuses>(entity =>
        {
            entity.HasKey(e => e.StatusCode);

            entity.Property(e => e.StatusCode).HasMaxLength(30);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
        });

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasIndex(e => new { e.CustomerId, e.OrderDate })
                .IsDescending(false, true)
                .HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.StatusCode).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.OrderNumber).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AdminNote).HasMaxLength(2000);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CustomerNote).HasMaxLength(1000);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RefundedAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.ShippingAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AppliedDiscount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AppliedDiscountId);

            entity.HasOne(d => d.BillingAddress).WithMany(p => p.OrdersBillingAddress)
                .HasForeignKey(d => d.BillingAddressId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrdersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ShippingAddress).WithMany(p => p.OrdersShippingAddress)
                .HasForeignKey(d => d.ShippingAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrdersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.OrdersUser)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Orders)
                .HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<PaymentGateways>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MethodCode).HasMaxLength(40);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Provider).HasMaxLength(80);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PaymentGatewaysCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.MethodCodeNavigation).WithMany(p => p.PaymentGateways)
                .HasForeignKey(d => d.MethodCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PaymentGatewaysUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<PaymentMethods>(entity =>
        {
            entity.HasKey(e => e.MethodCode);

            entity.Property(e => e.MethodCode).HasMaxLength(40);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsOnline).HasDefaultValue(true);
        });

        modelBuilder.Entity<PaymentStatuses>(entity =>
        {
            entity.HasKey(e => e.StatusCode);

            entity.Property(e => e.StatusCode).HasMaxLength(30);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
        });

        modelBuilder.Entity<Payments>(entity =>
        {
            entity.HasIndex(e => e.OrderId).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.TransactionId)
                .IsUnique()
                .HasFilter("([TransactionId] IS NOT NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValue("BDT")
                .IsFixedLength();
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.GatewayFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MethodCode).HasMaxLength(40);
            entity.Property(e => e.Provider).HasMaxLength(60);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            entity.Property(e => e.RefundedAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.StatusCode)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransactionAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionId).HasMaxLength(250);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PaymentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.MethodCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.MethodCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PaymentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<Permissions>(entity =>
        {
            entity.HasIndex(e => e.PermissionCode).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Module).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PermissionCode).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PermissionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PermissionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<PickupPoints>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ClosingTime).HasPrecision(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.OpeningTime).HasPrecision(0);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PickupPointsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PickupPointsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PickupPoints)
                .HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<PosCounters>(entity =>
        {
            entity.HasIndex(e => new { e.WarehouseId, e.CounterCode }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CounterCode).HasMaxLength(50);
            entity.Property(e => e.CounterName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosCountersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosCountersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PosCounters)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PosPaymentTenders>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CardLast4)
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.MethodCode).HasMaxLength(40);
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TransactionNo).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosPaymentTendersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.MethodCodeNavigation).WithMany(p => p.PosPaymentTenders)
                .HasForeignKey(d => d.MethodCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Transaction).WithMany(p => p.PosPaymentTenders)
                .HasForeignKey(d => d.TransactionId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosPaymentTendersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<PosTerminals>(entity =>
        {
            entity.HasIndex(e => new { e.PosCounterId, e.TerminalCode }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MachineName).HasMaxLength(100);
            entity.Property(e => e.PrinterName).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TerminalCode).HasMaxLength(50);
            entity.Property(e => e.TerminalName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTerminalsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.PosCounter).WithMany(p => p.PosTerminals)
                .HasForeignKey(d => d.PosCounterId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosTerminalsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<PosTransactionBundleSelections>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PriceAdjustment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionBundleSelections)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Group).WithMany(p => p.PosTransactionBundleSelections)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PosTransactionLine).WithMany(p => p.PosTransactionBundleSelections)
                .HasForeignKey(d => d.PosTransactionLineId);

            entity.HasOne(d => d.Variant).WithMany(p => p.PosTransactionBundleSelections)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PosTransactionLineTaxes>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxName).HasMaxLength(100);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(9, 4)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionLineTaxes)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.PosTransactionLine).WithMany(p => p.PosTransactionLineTaxes)
                .HasForeignKey(d => d.PosTransactionLineId);

            entity.HasOne(d => d.TaxRateNavigation).WithMany(p => p.PosTransactionLineTaxes)
                .HasForeignKey(d => d.TaxRateId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PosTransactionLines>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Barcode).HasMaxLength(60);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Batch).WithMany(p => p.PosTransactionLines)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionLinesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.PosTransactionLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Transaction).WithMany(p => p.PosTransactionLines)
                .HasForeignKey(d => d.TransactionId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosTransactionLinesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.PosTransactionLines)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<PosTransactionReturnLines>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Batch).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.PosTransactionReturn).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.PosTransactionReturnId);

            entity.HasOne(d => d.Product).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Variant).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<PosTransactionReturns>(entity =>
        {
            entity.HasIndex(e => e.ReturnNo).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.ReturnDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReturnNo).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionReturnsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PosTransactionReturnsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId);

            entity.HasOne(d => d.Customer).WithMany(p => p.PosTransactionReturns)
                .HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.Sale).WithMany(p => p.PosTransactionReturns)
                .HasForeignKey(d => d.SaleId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosTransactionReturnsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PosTransactionReturns)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PosTransactions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Sales");

            entity.HasIndex(e => new { e.CashierId, e.SaleDate })
                .IsDescending(false, true)
                .HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.CustomerId).HasFilter("([CustomerId] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.CashShiftId).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => new { e.WarehouseId, e.SaleDate }).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.ReceiptNumber).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ChangeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CouponCode).HasMaxLength(60);
            entity.Property(e => e.CouponDiscount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CustomerName).HasMaxLength(150);
            entity.Property(e => e.CustomerPhone).HasMaxLength(30);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReceiptNumber).HasMaxLength(50);
            entity.Property(e => e.RoundOffAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SaleDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.SaleType)
                .HasMaxLength(20)
                .HasDefaultValue("Regular");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Completed");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalItemQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VoidReason).HasMaxLength(250);

            entity.HasOne(d => d.AppliedDiscount).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.AppliedDiscountId);

            entity.HasOne(d => d.CashShift).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.CashShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CashierEmployee).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.CashierEmployeeId);

            entity.HasOne(d => d.Cashier).WithMany(p => p.PosTransactionsCashier)
                .HasForeignKey(d => d.CashierId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.PosCounter).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.PosCounterId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PosTerminal).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.PosTerminalId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosTransactionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.VoidedByNavigation).WithMany(p => p.PosTransactionsVoidedByNavigation)
                .HasForeignKey(d => d.VoidedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PriceListItems>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.EffectiveDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.MaxQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PriceListItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.PriceList).WithMany(p => p.PriceListItems)
                .HasForeignKey(d => d.PriceListId);

            entity.HasOne(d => d.Product).WithMany(p => p.PriceListItems)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PriceListItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.PriceListItems)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<PriceLists>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PriceListType)
                .HasMaxLength(30)
                .HasDefaultValue("CustomerGroup");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TierCode).HasMaxLength(20);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PriceListsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.TierCodeNavigation).WithMany(p => p.PriceLists)
                .HasForeignKey(d => d.TierCode);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PriceListsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ProductAttributeLinks>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.AttributeTypeId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsRequired).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.AttributeType).WithMany(p => p.ProductAttributeLinks)
                .HasForeignKey(d => d.AttributeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductAttributeLinks)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductAttributeLinks)
                .HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<ProductBatches>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.BatchNo }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BatchNo).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductBatchesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductBatches)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductBatchesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ProductCollectionItems>(entity =>
        {
            entity.HasIndex(e => new { e.ProductCollectionId, e.ProductId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductCollectionItems)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.ProductCollection).WithMany(p => p.ProductCollectionItems)
                .HasForeignKey(d => d.ProductCollectionId);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCollectionItems)
                .HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<ProductCollections>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Collections");

            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(150);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductCollectionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductCollectionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ProductConditions>(entity =>
        {
            entity.HasKey(e => e.ConditionCode);

            entity.Property(e => e.ConditionCode).HasMaxLength(20);
            entity.Property(e => e.DisplayName).HasMaxLength(50);
        });

        modelBuilder.Entity<ProductImages>(entity =>
        {
            entity.HasIndex(e => e.ProductId).HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AltText).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductImagesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductImagesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<ProductMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Media");

            entity.HasIndex(e => new { e.ProductId, e.Scope, e.SortOrder }).HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AltText).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Etag)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("ETag");
            entity.Property(e => e.FileName).HasMaxLength(260);
            entity.Property(e => e.MediaType)
                .HasMaxLength(10)
                .HasDefaultValue("Image");
            entity.Property(e => e.MimeType).HasMaxLength(80);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Scope)
                .HasMaxLength(20)
                .HasDefaultValue("Product");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductMediaCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductMedia)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductMediaUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.ProductMediaUploadedByNavigation)
                .HasForeignKey(d => d.UploadedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductMedia)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<ProductMediaBlob>(entity =>
        {
            entity.HasKey(e => e.MediaId);

            entity.Property(e => e.MediaId).ValueGeneratedNever();

            entity.HasOne(d => d.Media).WithOne(p => p.ProductMediaBlob)
                .HasForeignKey<ProductMediaBlob>(d => d.MediaId);
        });

        modelBuilder.Entity<ProductPriceHistories>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.EffectiveFrom }).IsDescending(false, true);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.EffectiveFrom).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.NewCostPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NewSalePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OldCostPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OldSalePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.ProductPriceHistoriesChangedByUser)
                .HasForeignKey(d => d.ChangedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductPriceHistoriesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPriceHistories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProductSpecificationValues>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductSpecificationValuesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSpecificationValues)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Spec).WithMany(p => p.ProductSpecificationValues)
                .HasForeignKey(d => d.SpecId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductSpecificationValuesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductSpecificationValues)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<ProductSpecifications>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Specs");

            entity.HasIndex(e => e.SpecName).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SpecName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductSpecifications)
                .HasForeignKey(d => d.CreatedBy);
        });

        modelBuilder.Entity<ProductSupplierLinks>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductSuppliers");

            entity.HasIndex(e => new { e.ProductId, e.SupplierId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SupplierSku)
                .HasMaxLength(50)
                .HasColumnName("SupplierSKU");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductSupplierLinksCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSupplierLinks)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Supplier).WithMany(p => p.ProductSupplierLinks)
                .HasForeignKey(d => d.SupplierId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductSupplierLinksUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ProductTags>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.TagId });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Tag).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.TagId);
        });

        modelBuilder.Entity<ProductTaxRates>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductTax");

            entity.HasIndex(e => new { e.ProductId, e.TaxRateId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductTaxRatesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTaxRates)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.TaxRate).WithMany(p => p.ProductTaxRates)
                .HasForeignKey(d => d.TaxRateId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductTaxRatesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ProductVariants>(entity =>
        {
            entity.HasIndex(e => e.ProductId).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Barcode)
                .IsUnique()
                .HasFilter("([Barcode] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.Sku)
                .IsUnique()
                .HasFilter("([SKU] IS NOT NULL AND [IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Barcode).HasMaxLength(50);
            entity.Property(e => e.CostPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OverridePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PriceModifier).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.WeightKg).HasColumnType("decimal(8, 3)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductVariantsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductVariantsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasIndex(e => new { e.IsActive, e.ProductType }).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.BrandId).HasFilter("([BrandId] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.CategoryId).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Barcode)
                .IsUnique()
                .HasFilter("([Barcode] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.Sku)
                .IsUnique()
                .HasFilter("([SKU] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.ConditionCode).HasMaxLength(20);
            entity.Property(e => e.CostPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Dimensions).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxSaleQty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MetaDescription).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(200);
            entity.Property(e => e.MinSaleQty)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.ProductType)
                .HasMaxLength(20)
                .HasDefaultValue("Simple");
            entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ShelfLocation).HasMaxLength(100);
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
            entity.Property(e => e.ShortName).HasMaxLength(100);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.Property(e => e.WeightKg).HasColumnType("decimal(8, 3)");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Color).WithMany(p => p.Products)
                .HasForeignKey(d => d.ColorId);

            entity.HasOne(d => d.ConditionCodeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ConditionCode);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Seller).WithMany(p => p.Products)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.TaxRate).WithMany(p => p.Products)
                .HasForeignKey(d => d.TaxRateId);

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<PurchaseOrderLineTaxes>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxName).HasMaxLength(100);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(9, 4)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseOrderLineTaxes)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.PurchaseOrderLine).WithMany(p => p.PurchaseOrderLineTaxes)
                .HasForeignKey(d => d.PurchaseOrderLineId);

            entity.HasOne(d => d.TaxRateNavigation).WithMany(p => p.PurchaseOrderLineTaxes)
                .HasForeignKey(d => d.TaxRateId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PurchaseOrderLines>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PurchaseLines");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReceivedQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Batch).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseOrderLinesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.PurchaseOrderId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PurchaseOrderLinesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<PurchaseOrders>(entity =>
        {
            entity.HasIndex(e => e.OrderNumber).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DueAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InvoiceNo).HasMaxLength(100);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.OtherCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RoundOffAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(25)
                .HasDefaultValue("Draft");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalItemQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalTaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransportCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.PurchaseOrdersApprovedByUser)
                .HasForeignKey(d => d.ApprovedByUserId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseOrdersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PurchaseOrdersCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PurchaseOrdersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<PurchaseReturnLines>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Batch).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PurchaseReturn).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.PurchaseReturnId);

            entity.HasOne(d => d.Variant).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<PurchaseReturns>(entity =>
        {
            entity.HasIndex(e => e.ReturnNo).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.ReturnDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReturnNo).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseReturnsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PurchaseReturnsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId);

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PurchaseReturnsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<QuoteItems>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QuoteItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Quote).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.QuoteId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.QuoteItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<Quotes>(entity =>
        {
            entity.HasIndex(e => e.QuoteNo).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuoteDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.QuoteNo).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QuotesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.Order).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.QuotesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RefundRequests>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Refunds");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.StatusCode)
                .HasMaxLength(30)
                .HasDefaultValue("Requested");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RefundRequestsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.Order).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Return).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.ReturnId);

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RefundRequestsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ReorderRules>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReorderQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ReorderRulesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.NotifyUser).WithMany(p => p.ReorderRulesNotifyUser)
                .HasForeignKey(d => d.NotifyUserId);

            entity.HasOne(d => d.PreferredSupplier).WithMany(p => p.ReorderRules)
                .HasForeignKey(d => d.PreferredSupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Product).WithMany(p => p.ReorderRules)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ReorderRulesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.ReorderRules)
                .HasForeignKey(d => d.VariantId);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.ReorderRules)
                .HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<ReturnStatuses>(entity =>
        {
            entity.HasKey(e => e.StatusCode);

            entity.Property(e => e.StatusCode).HasMaxLength(30);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
        });

        modelBuilder.Entity<Returns>(entity =>
        {
            entity.HasIndex(e => e.ReturnNumber).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RefundMethodCode).HasMaxLength(40);
            entity.Property(e => e.RequestDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReturnNumber).HasMaxLength(50);
            entity.Property(e => e.RmaNumber).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.StatusCode)
                .HasMaxLength(30)
                .HasDefaultValue("Requested");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ReturnsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Order).WithMany(p => p.Returns)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProcessedByUser).WithMany(p => p.ReturnsProcessedByUser)
                .HasForeignKey(d => d.ProcessedByUserId);

            entity.HasOne(d => d.RefundMethodCodeNavigation).WithMany(p => p.Returns)
                .HasForeignKey(d => d.RefundMethodCode);

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Returns)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ReturnsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ReviewHelpfulness>(entity =>
        {
            entity.HasKey(e => new { e.ReviewId, e.UserId });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewHelpfulness)
                .HasForeignKey(d => d.ReviewId);

            entity.HasOne(d => d.User).WithMany(p => p.ReviewHelpfulness)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Reviews>(entity =>
        {
            entity.HasIndex(e => e.ProductId).HasFilter("([IsDeleted]=(0) AND [IsApproved]=(1))");

            entity.HasIndex(e => new { e.CustomerId, e.ProductId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ReviewsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Order).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ReviewsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<RoleClaims>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims)
                .HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<RoleMenus>(entity =>
        {
            entity.HasIndex(e => new { e.RoleId, e.MenuId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CanView).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoleMenusCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus)
                .HasForeignKey(d => d.MenuId);

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus)
                .HasForeignKey(d => d.RoleId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RoleMenusUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<RolePermissions>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });

            entity.Property(e => e.IsGranted).HasDefaultValue(true);

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId);

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName)
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<SearchKeywords>(entity =>
        {
            entity.HasIndex(e => e.Keyword).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Keyword).HasMaxLength(200);
            entity.Property(e => e.LastSearchedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SearchCount).HasDefaultValue(1);
        });

        modelBuilder.Entity<Sellers>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CommissionRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .HasDefaultValue("BD")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.Property(e => e.StoreBanner).HasMaxLength(500);
            entity.Property(e => e.StoreLogo).HasMaxLength(500);
            entity.Property(e => e.StoreName).HasMaxLength(200);

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.SellersApprovedByUser)
                .HasForeignKey(d => d.ApprovedByUserId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SellersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SellersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithOne(p => p.SellersUser)
                .HasForeignKey<Sellers>(d => d.UserId);
        });

        modelBuilder.Entity<ShipmentStatuses>(entity =>
        {
            entity.HasKey(e => e.StatusCode);

            entity.Property(e => e.StatusCode).HasMaxLength(30);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
        });

        modelBuilder.Entity<Shipments>(entity =>
        {
            entity.HasIndex(e => e.OrderId).HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DeliveryNotes).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.ShippingCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TrackingNumber).HasMaxLength(120);
            entity.Property(e => e.TrackingUrl).HasMaxLength(500);
            entity.Property(e => e.WeightKg).HasColumnType("decimal(8, 3)");

            entity.HasOne(d => d.Carrier).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.CarrierId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ShipmentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Order).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.ShippingMethod).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.ShippingMethodId);

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ShipmentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<ShippingCarriers>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BaseCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ShippingCarriersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ShippingCarriersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<ShippingMethods>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BaseCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CarrierName).HasMaxLength(100);
            entity.Property(e => e.CostPerKg).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.EstimatedDaysMax).HasDefaultValue(7);
            entity.Property(e => e.EstimatedDaysMin).HasDefaultValue(1);
            entity.Property(e => e.FreeShippingThreshold).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ShippingMethodsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ShippingMethodsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<StaticPages>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Pages");

            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsPublished).HasDefaultValue(true);
            entity.Property(e => e.MetaDescription).HasMaxLength(500);
            entity.Property(e => e.MetaTitle).HasMaxLength(200);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StaticPagesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.StaticPagesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<StockItems>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Stock");

            entity.HasIndex(e => new { e.ProductId, e.WarehouseId }).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.WarehouseId).HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => new { e.ProductId, e.VariantId, e.BatchId, e.WarehouseId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AverageCostPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.QuantityOnHand).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReservedQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Batch).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CountedByUser).WithMany(p => p.StockItemsCountedByUser)
                .HasForeignKey(d => d.CountedByUserId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.StockItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.VariantId);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StockMovementTypes>(entity =>
        {
            entity.HasKey(e => e.TypeCode);

            entity.Property(e => e.TypeCode).HasMaxLength(30);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
            entity.Property(e => e.IsInbound).HasDefaultValue(true);
        });

        modelBuilder.Entity<StockMovements>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.OccurredAt })
                .IsDescending(false, true)
                .HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BalanceAfter).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.MovementTypeCode).HasMaxLength(30);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.QuantityIn).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuantityOut).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
            entity.Property(e => e.ReferenceType).HasMaxLength(40);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Batch).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.StockMovementsFromWarehouse)
                .HasForeignKey(d => d.FromWarehouseId);

            entity.HasOne(d => d.MovementTypeCodeNavigation).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.MovementTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Product).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StockItem).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.StockItemId);

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.StockMovementsToWarehouse)
                .HasForeignKey(d => d.ToWarehouseId);

            entity.HasOne(d => d.Variant).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<StockTransferLines>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Batch).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.BatchId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Transfer).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.TransferId);

            entity.HasOne(d => d.Variant).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<StockTransfers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Transfers");

            entity.HasIndex(e => e.TransferNo).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransferDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.TransferNo).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockTransfersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.StockTransfersCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId);

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.StockTransfersFromWarehouse)
                .HasForeignKey(d => d.FromWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.StockTransfersToWarehouse)
                .HasForeignKey(d => d.ToWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.StockTransfersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<Suppliers>(entity =>
        {
            entity.HasIndex(e => e.SupplierCode).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.AddressLine2).HasMaxLength(200);
            entity.Property(e => e.AlternatePhone).HasMaxLength(30);
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(150);
            entity.Property(e => e.ContactPerson).HasMaxLength(150);
            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .HasDefaultValue("BD")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.PaymentTerms).HasMaxLength(80);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.SupplierCode).HasMaxLength(50);
            entity.Property(e => e.SupplierType).HasMaxLength(50);
            entity.Property(e => e.TaxRegistrationNo).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SuppliersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SuppliersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<SupportTicketMessages>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SupportTicketMessagesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Sender).WithMany(p => p.SupportTicketMessagesSender)
                .HasForeignKey(d => d.SenderId);

            entity.HasOne(d => d.SupportTicket).WithMany(p => p.SupportTicketMessages)
                .HasForeignKey(d => d.SupportTicketId);
        });

        modelBuilder.Entity<SupportTickets>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tickets");

            entity.HasIndex(e => e.TicketNumber).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasDefaultValue("Normal");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.TicketNumber).HasMaxLength(30);

            entity.HasOne(d => d.AssignedTo).WithMany(p => p.SupportTicketsAssignedTo)
                .HasForeignKey(d => d.AssignedToId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SupportTicketsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Order).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SupportTicketsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.SupportTicketsUser)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Tags>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TagsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TagsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<TaxRates>(entity =>
        {
            entity.HasIndex(e => e.TaxCode).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .HasDefaultValue("BD")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPercentage).HasDefaultValue(true);
            entity.Property(e => e.Rate).HasColumnType("decimal(9, 4)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TaxCode).HasMaxLength(50);
            entity.Property(e => e.TaxName).HasMaxLength(100);
            entity.Property(e => e.TaxType)
                .HasMaxLength(30)
                .HasDefaultValue("Percentage");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaxRatesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TaxRatesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<Units>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ConversionFactor).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.ShortName).HasMaxLength(20);

            entity.HasOne(d => d.BaseUnit).WithMany(p => p.InverseBaseUnit)
                .HasForeignKey(d => d.BaseUnitId);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UnitsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.UnitsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<UserClaims>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.User).WithMany(p => p.UserClaims)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserLogins>(entity =>
        {
            entity.HasIndex(e => new { e.LoginProvider, e.ProviderKey }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserRefreshTokens>(entity =>
        {
            entity.HasIndex(e => e.UserId).HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedByIp).HasMaxLength(50);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.RevokedByIp).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserRefreshTokensCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.UserRefreshTokensUser)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserTokens>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.LoginProvider, e.Name }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserRoles>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Role)
                .WithMany()
                .HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail)
                .IsUnique()
                .HasFilter("([NormalizedEmail] IS NOT NULL)");

            entity.HasIndex(e => e.NormalizedUserName)
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.LockoutEnabled).HasDefaultValue(true);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.PreferredLanguage)
                .HasMaxLength(5)
                .HasDefaultValue("en")
                .IsFixedLength();
            entity.Property(e => e.TimeZone)
                .HasMaxLength(60)
                .HasDefaultValue("Asia/Dhaka");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<VariantAttributeMatrix>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.VariantId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.VariantAttributeMatrix)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.VariantAttributeMatrix)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantAttributeMatrix)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<VariantAttributeOptions>(entity =>
        {
            entity.HasIndex(e => new { e.VariantId, e.OptionId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.VariantAttributeOptions)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Option).WithMany(p => p.VariantAttributeOptions)
                .HasForeignKey(d => d.OptionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantAttributeOptions)
                .HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<VwCustomerLoyaltyBalance>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CustomerLoyaltyBalance");
        });

        modelBuilder.Entity<VwCustomerStats>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CustomerStats");

            entity.Property(e => e.TotalRefunded).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.TotalSpent).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwProductStats>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ProductStats");

            entity.Property(e => e.RatingAverage).HasColumnType("decimal(3, 2)");
        });

        modelBuilder.Entity<VwStockAvailability>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StockAvailability");

            entity.Property(e => e.AvailableQty).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.AverageCostPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.QuantityOnHand).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReservedQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SiteType).HasMaxLength(20);
            entity.Property(e => e.VariantName).HasMaxLength(100);
            entity.Property(e => e.WarehouseName).HasMaxLength(150);
        });

        modelBuilder.Entity<WalletTransactions>(entity =>
        {
            entity.HasIndex(e => new { e.WalletId, e.CreatedAt }).IsDescending(false, true);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BalanceAfter).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Reference).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Completed");
            entity.Property(e => e.TransactionType).HasMaxLength(30);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WalletTransactionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WalletTransactionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.WalletId);
        });

        modelBuilder.Entity<Warehouses>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddressLine1).HasMaxLength(200);
            entity.Property(e => e.AddressLine2).HasMaxLength(200);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ClosingTime).HasPrecision(0);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.ContactPerson).HasMaxLength(150);
            entity.Property(e => e.Country)
                .HasMaxLength(2)
                .HasDefaultValue("BD")
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.ManagerName).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.OpeningTime).HasPrecision(0);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SiteType)
                .HasMaxLength(20)
                .HasDefaultValue("Warehouse");
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.TaxNumber).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WarehousesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WarehousesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);
        });

        modelBuilder.Entity<WishlistItems>(entity =>
        {
            entity.HasIndex(e => new { e.WishlistId, e.ProductId, e.VariantId }).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WishlistItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Product).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WishlistItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.Variant).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.VariantId);

            entity.HasOne(d => d.Wishlist).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.WishlistId);
        });

        modelBuilder.Entity<WishlistTypes>(entity =>
        {
            entity.HasKey(e => e.TypeCode);

            entity.Property(e => e.TypeCode).HasMaxLength(20);
            entity.Property(e => e.DisplayName).HasMaxLength(50);
        });

        modelBuilder.Entity<Wishlists>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasDefaultValue("My Wishlist");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SharingToken).HasMaxLength(100);
            entity.Property(e => e.WishlistTypeCode)
                .HasMaxLength(20)
                .HasDefaultValue("Personal");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WishlistsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Customer).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WishlistsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy);

            entity.HasOne(d => d.User).WithMany(p => p.WishlistsUser)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.WishlistTypeCodeNavigation).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.WishlistTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
