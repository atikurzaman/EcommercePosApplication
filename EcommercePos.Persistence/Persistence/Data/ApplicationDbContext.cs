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
            entity.HasIndex(e => new { e.UserId, e.OccurredAt }, "IX_ActivityLog_User")
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
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ActivityL__UserI__6F8A7843");
        });

        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasIndex(e => e.Key, "UX_AppSettings_Key").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__AppSettin__Creat__3EE740E8");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AppSettingsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__AppSettin__Updat__3FDB6521");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Attribute__Creat__119F9925");

            entity.HasOne(d => d.Option).WithMany(p => p.AttributeOptionMedia)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("FK__Attribute__Optio__0CDAE408");

            entity.HasOne(d => d.Product).WithMany(p => p.AttributeOptionMedia)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Attribute__Produ__0DCF0841");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AttributeOptionMediaUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Attribute__Updat__1293BD5E");
        });

        modelBuilder.Entity<AttributeOptionMediaBlob>(entity =>
        {
            entity.HasKey(e => e.MediaId);

            entity.Property(e => e.MediaId).ValueGeneratedNever();

            entity.HasOne(d => d.Media).WithOne(p => p.AttributeOptionMediaBlob)
                .HasForeignKey<AttributeOptionMediaBlob>(d => d.MediaId)
                .HasConstraintName("FK_AttributeOptionMediaBlob");
        });

        modelBuilder.Entity<AttributeOptions>(entity =>
        {
            entity.HasIndex(e => new { e.AttributeTypeId, e.Value }, "UX_AttributeOptions").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DisplayValue).HasMaxLength(120);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Value).HasMaxLength(120);

            entity.HasOne(d => d.AttributeType).WithMany(p => p.AttributeOptions)
                .HasForeignKey(d => d.AttributeTypeId)
                .HasConstraintName("FK__Attribute__Attri__54CB950F");

            entity.HasOne(d => d.Color).WithMany(p => p.AttributeOptions)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("FK__Attribute__Color__55BFB948");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AttributeOptionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Attribute__Creat__59904A2C");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AttributeOptionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Attribute__Updat__5A846E65");
        });

        modelBuilder.Entity<AttributeTypes>(entity =>
        {
            entity.HasIndex(e => e.Name, "UX_AttributeTypes_Name").IsUnique();

            entity.HasIndex(e => e.Slug, "UX_AttributeTypes_Slug").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Attribute__Creat__4E1E9780");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AttributeTypesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Attribute__Updat__4F12BBB9");
        });

        modelBuilder.Entity<AuditLogs>(entity =>
        {
            entity.HasIndex(e => e.OccurredAt, "IX_AuditLog_Date").IsDescending();

            entity.HasIndex(e => new { e.EntityName, e.EntityId }, "IX_AuditLog_Entity");

            entity.HasIndex(e => new { e.UserId, e.OccurredAt }, "IX_AuditLog_User").IsDescending(false, true);

            entity.Property(e => e.Action).HasMaxLength(120);
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.EntityId).HasMaxLength(60);
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.OccurredAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__AuditLogs__UserI__744F2D60");
        });

        modelBuilder.Entity<BlogCategories>(entity =>
        {
            entity.HasIndex(e => e.Slug, "UX_BlogCats_Slug").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogCategoriesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__BlogCateg__Creat__0A7378A9");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BlogCategoriesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__BlogCateg__Updat__0B679CE2");
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
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK__BlogComme__BlogI__261B931E");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogCommentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__BlogComme__Creat__2AE0483B");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__BlogComme__Paren__2803DB90");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BlogCommentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__BlogComme__Updat__2BD46C74");

            entity.HasOne(d => d.User).WithMany(p => p.BlogCommentsUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__BlogComme__UserI__270FB757");
        });

        modelBuilder.Entity<BlogTags>(entity =>
        {
            entity.HasIndex(e => e.Slug, "UX_BlogTags_Slug").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(80);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(80);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogTags)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__BlogTags__Create__1D864D1D");
        });

        modelBuilder.Entity<Blogs>(entity =>
        {
            entity.HasIndex(e => e.Slug, "UX_Blogs_Slug").IsUnique();

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
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Blogs__AuthorId__12149A71");

            entity.HasOne(d => d.Category).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Blogs__CategoryI__11207638");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Blogs__CreatedBy__15E52B55");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BlogsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Blogs__UpdatedBy__16D94F8E");

            entity.HasMany(d => d.BlogTag).WithMany(p => p.Blog)
                .UsingEntity<Dictionary<string, object>>(
                    "BlogPostTags",
                    r => r.HasOne<BlogTags>().WithMany()
                        .HasForeignKey("BlogTagId")
                        .HasConstraintName("FK__BlogPostT__BlogT__224B023A"),
                    l => l.HasOne<Blogs>().WithMany()
                        .HasForeignKey("BlogId")
                        .HasConstraintName("FK__BlogPostT__BlogI__2156DE01"),
                    j =>
                    {
                        j.HasKey("BlogId", "BlogTagId");
                    });
        });

        modelBuilder.Entity<Brands>(entity =>
        {
            entity.HasIndex(e => e.Slug, "UX_Brands_Slug").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Brands__CreatedB__690797E6");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BrandsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Brands__UpdatedB__69FBBC1F");

            entity.HasMany(d => d.Category).WithMany(p => p.Brand)
                .UsingEntity<Dictionary<string, object>>(
                    "BrandCategories",
                    r => r.HasOne<Categories>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK__BrandCate__Categ__1E6F845E"),
                    l => l.HasOne<Brands>().WithMany()
                        .HasForeignKey("BrandId")
                        .HasConstraintName("FK__BrandCate__Brand__1D7B6025"),
                    j =>
                    {
                        j.HasKey("BrandId", "CategoryId");
                    });
        });

        modelBuilder.Entity<BundleComponents>(entity =>
        {
            entity.HasIndex(e => new { e.BundleProductId, e.ComponentVariantId }, "UX_BundleComponents").IsUnique();

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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BundleCom__Bundl__1B29035F");

            entity.HasOne(d => d.ComponentVariant).WithMany(p => p.BundleComponents)
                .HasForeignKey(d => d.ComponentVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BundleCom__Compo__1C1D2798");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BundleComponentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__BundleCom__Creat__21D600EE");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BundleComponentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__BundleCom__Updat__22CA2527");
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
                .HasForeignKey(d => d.BundleProductId)
                .HasConstraintName("FK__BundleOpt__Bundl__278EDA44");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BundleOptionGroupsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__BundleOpt__Creat__2E3BD7D3");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BundleOptionGroupsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__BundleOpt__Updat__2F2FFC0C");
        });

        modelBuilder.Entity<BundleOptionItems>(entity =>
        {
            entity.HasIndex(e => new { e.GroupId, e.VariantId }, "UX_BundleOptionItems").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PriceAdjustment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BundleOptionItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__BundleOpt__Creat__3AA1AEB8");

            entity.HasOne(d => d.Group).WithMany(p => p.BundleOptionItems)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__BundleOpt__Group__34E8D562");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BundleOptionItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__BundleOpt__Updat__3B95D2F1");

            entity.HasOne(d => d.Variant).WithMany(p => p.BundleOptionItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BundleOpt__Varia__35DCF99B");
        });

        modelBuilder.Entity<CartItems>(entity =>
        {
            entity.HasIndex(e => e.CartId, "IX_CartItems_Cart").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.ProductId, "IX_CartItems_Product").HasFilter("([IsDeleted]=(0))");

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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__CartItems__Batch__6CF8245B");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__CartItems__CartI__6A1BB7B0");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CartItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__CartItems__Creat__70C8B53F");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItems__Produ__6B0FDBE9");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CartItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__CartItems__Updat__71BCD978");

            entity.HasOne(d => d.Variant).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__CartItems__Varia__6C040022");
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
                .HasForeignKey(d => d.AppliedDiscountId)
                .HasConstraintName("FK__Carts__AppliedDi__618671AF");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CartsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Carts__CreatedBy__636EBA21");

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Carts__CustomerI__5CC1BC92");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CartsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Carts__UpdatedBy__6462DE5A");

            entity.HasOne(d => d.User).WithMany(p => p.CartsUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Carts__UserId__5DB5E0CB");
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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CashDrawe__CashS__4B8221F7");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CashDrawerEventsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__CashDrawe__Creat__513AFB4D");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.CashDrawerEventsPerformedByNavigation)
                .HasForeignKey(d => d.PerformedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CashDrawe__Perfo__4C764630");

            entity.HasOne(d => d.Transaction).WithMany(p => p.CashDrawerEvents)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK__CashDrawe__Trans__4D6A6A69");
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
                .HasForeignKey(d => d.ClosedByEmployeeId)
                .HasConstraintName("FK__CashShift__Close__76A18A26");

            entity.HasOne(d => d.ClosedByUser).WithMany(p => p.CashShiftsClosedByUser)
                .HasForeignKey(d => d.ClosedByUserId)
                .HasConstraintName("FK__CashShift__Close__7889D298");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CashShiftsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__CashShift__Creat__002AF460");

            entity.HasOne(d => d.OpenedByEmployee).WithMany(p => p.CashShiftsOpenedByEmployee)
                .HasForeignKey(d => d.OpenedByEmployeeId)
                .HasConstraintName("FK__CashShift__Opene__75AD65ED");

            entity.HasOne(d => d.OpenedByUser).WithMany(p => p.CashShiftsOpenedByUser)
                .HasForeignKey(d => d.OpenedByUserId)
                .HasConstraintName("FK__CashShift__Opene__7795AE5F");

            entity.HasOne(d => d.PosCounter).WithMany(p => p.CashShifts)
                .HasForeignKey(d => d.PosCounterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CashShift__PosCo__73C51D7B");

            entity.HasOne(d => d.PosTerminal).WithMany(p => p.CashShifts)
                .HasForeignKey(d => d.PosTerminalId)
                .HasConstraintName("FK__CashShift__PosTe__74B941B4");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CashShiftsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__CashShift__Updat__011F1899");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.CashShifts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CashShift__Wareh__72D0F942");
        });

        modelBuilder.Entity<Categories>(entity =>
        {
            entity.HasIndex(e => e.ParentCategoryId, "IX_Categories_Parent").HasFilter("([ParentCategoryId] IS NOT NULL)");

            entity.HasIndex(e => e.Slug, "UX_Categories_Slug").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Categorie__Creat__74794A92");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK__Categorie__Paren__6FB49575");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CategoriesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Categorie__Updat__756D6ECB");
        });

        modelBuilder.Entity<Colors>(entity =>
        {
            entity.HasIndex(e => e.Name, "UX_Colors_Name").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Colors__CreatedB__078C1F06");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ColorsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Colors__UpdatedB__0880433F");
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
                .HasForeignKey(d => d.RepliedBy)
                .HasConstraintName("FK__ContactMe__Repli__4F1DA8B1");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Currencie__Creat__41EDCAC5");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CurrenciesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Currencie__Updat__42E1EEFE");
        });

        modelBuilder.Entity<CustomerAddresses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Addresses");

            entity.HasIndex(e => e.CustomerId, "IX_Addresses_Customer").HasFilter("([IsDeleted]=(0))");

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__CustomerA__Creat__3F3159AB");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerAddresses)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__CustomerA__Custo__38845C1C");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerAddressesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__CustomerA__Updat__40257DE4");

            entity.HasOne(d => d.User).WithMany(p => p.CustomerAddressesUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__CustomerA__UserI__39788055");
        });

        modelBuilder.Entity<CustomerProfiles>(entity =>
        {
            entity.HasIndex(e => e.CustomerId, "UX_CustomerProfiles").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TierCode).HasMaxLength(20);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerProfilesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__CustomerP__Creat__17236851");

            entity.HasOne(d => d.Customer).WithOne(p => p.CustomerProfiles)
                .HasForeignKey<CustomerProfiles>(d => d.CustomerId)
                .HasConstraintName("FK__CustomerP__Custo__125EB334");

            entity.HasOne(d => d.TierCodeNavigation).WithMany(p => p.CustomerProfiles)
                .HasForeignKey(d => d.TierCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerP__TierC__1352D76D");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerProfilesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__CustomerP__Updat__18178C8A");
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

            entity.HasIndex(e => e.CustomerId, "UX_Wallets_Customer").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__CustomerW__Creat__4C8B54C9");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.CustomerWallets)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerW__Curre__47C69FAC");

            entity.HasOne(d => d.Customer).WithOne(p => p.CustomerWallets)
                .HasForeignKey<CustomerWallets>(d => d.CustomerId)
                .HasConstraintName("FK__CustomerW__Custo__45DE573A");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomerWalletsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__CustomerW__Updat__4D7F7902");
        });

        modelBuilder.Entity<Customers>(entity =>
        {
            entity.HasIndex(e => e.CustomerCode, "UX_Customers_Code").IsUnique();

            entity.HasIndex(e => e.ReferralCode, "UX_Customers_ReferralCode")
                .IsUnique()
                .HasFilter("([ReferralCode] IS NOT NULL)");

            entity.HasIndex(e => e.UserId, "UX_Customers_UserId")
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Customers__Creat__0BB1B5A5");

            entity.HasOne(d => d.ReferredByCustomer).WithMany(p => p.InverseReferredByCustomer)
                .HasForeignKey(d => d.ReferredByCustomerId)
                .HasConstraintName("FK__Customers__Refer__07E124C1");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.CustomersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Customers__Updat__0CA5D9DE");

            entity.HasOne(d => d.User).WithOne(p => p.CustomersUser)
                .HasForeignKey<Customers>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Customers__UserI__02284B6B");
        });

        modelBuilder.Entity<DayEndSummaries>(entity =>
        {
            entity.HasIndex(e => new { e.SummaryDate, e.WarehouseId }, "UX_DayEndSummaries").IsUnique();

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
                .HasForeignKey(d => d.CashShiftId)
                .HasConstraintName("FK__DayEndSum__CashS__57E7F8DC");

            entity.HasOne(d => d.ClosedByUser).WithMany(p => p.DayEndSummariesClosedByUser)
                .HasForeignKey(d => d.ClosedByUserId)
                .HasConstraintName("FK__DayEndSum__Close__6CE315C2");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DayEndSummariesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__DayEndSum__Creat__6ECB5E34");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DayEndSummariesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__DayEndSum__Updat__6FBF826D");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.DayEndSummaries)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__DayEndSum__Wareh__56F3D4A3");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__DeliveryZ__Creat__0F183235");

            entity.HasOne(d => d.DeliveryZone).WithMany(p => p.DeliveryZoneRegions)
                .HasForeignKey(d => d.DeliveryZoneId)
                .HasConstraintName("FK__DeliveryZ__Deliv__0C3BC58A");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DeliveryZoneRegionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__DeliveryZ__Updat__100C566E");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__DeliveryZ__Creat__0682EC34");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DeliveryZonesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__DeliveryZ__Updat__0777106D");
        });

        modelBuilder.Entity<DiscountApplicability>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Category).WithMany(p => p.DiscountApplicability)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__DiscountA__Categ__3552E9B6");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountApplicability)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__DiscountA__Disco__345EC57D");

            entity.HasOne(d => d.Product).WithMany(p => p.DiscountApplicability)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DiscountApplicability_Products");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__DiscountU__Creat__3DE82FB7");

            entity.HasOne(d => d.Customer).WithMany(p => p.DiscountUsageLog)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_DiscountUsageLog_Customers");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountUsageLog)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__DiscountU__Disco__3A179ED3");

            entity.HasOne(d => d.Order).WithMany(p => p.DiscountUsageLog)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_DiscountUsageLog_Orders");

            entity.HasOne(d => d.PosTransaction).WithMany(p => p.DiscountUsageLog)
                .HasForeignKey(d => d.PosTransactionId)
                .HasConstraintName("FK_DiscountUsageLog_Sales");

            entity.HasOne(d => d.User).WithMany(p => p.DiscountUsageLogUser)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DiscountU__UserI__3B0BC30C");
        });

        modelBuilder.Entity<Discounts>(entity =>
        {
            entity.HasIndex(e => e.Code, "UX_Discounts_Code").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Discounts__Creat__2DB1C7EE");

            entity.HasOne(d => d.DiscountTypeCodeNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Discounts__Disco__2334397B");

            entity.HasOne(d => d.TierCodeNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.TierCode)
                .HasConstraintName("FK__Discounts__TierC__2704CA5F");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DiscountsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Discounts__Updat__2EA5EC27");
        });

        modelBuilder.Entity<EmailTemplates>(entity =>
        {
            entity.HasIndex(e => e.TemplateType, "UX_Templates_Type").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__EmailTemp__Creat__477C86E9");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.EmailTemplatesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__EmailTemp__Updat__4870AB22");
        });

        modelBuilder.Entity<Employees>(entity =>
        {
            entity.HasIndex(e => e.EmployeeCode, "UX_Employees_Code").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Employees__Creat__5A054B78");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.EmployeesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Employees__Updat__5AF96FB1");

            entity.HasOne(d => d.User).WithMany(p => p.EmployeesUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Employees__UserI__5728DECD");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Employees)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__Employees__Wareh__5634BA94");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ExpenseCa__Creat__766C7FFC");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ExpenseCategoriesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ExpenseCa__Updat__7760A435");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Expenses__Create__02D256E1");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ExpensesCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK__Expenses__Create__00EA0E6F");

            entity.HasOne(d => d.ExpenseCategory).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.ExpenseCategoryId)
                .HasConstraintName("FK__Expenses__Expens__7D197D8B");

            entity.HasOne(d => d.MethodCodeNavigation).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.MethodCode)
                .HasConstraintName("FK__Expenses__Method__7FF5EA36");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ExpensesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Expenses__Update__03C67B1A");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Expenses__Wareho__7C255952");
        });

        modelBuilder.Entity<FlashDealProducts>(entity =>
        {
            entity.HasIndex(e => new { e.FlashDealId, e.ProductId }, "UX_FlashDealProducts").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.DiscountType).HasDefaultValue((byte)1);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FlashDealProductsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__FlashDeal__Creat__7B7B4DDC");

            entity.HasOne(d => d.FlashDeal).WithMany(p => p.FlashDealProducts)
                .HasForeignKey(d => d.FlashDealId)
                .HasConstraintName("FK__FlashDeal__Flash__73DA2C14");

            entity.HasOne(d => d.Product).WithMany(p => p.FlashDealProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__FlashDeal__Produ__74CE504D");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.FlashDealProductsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__FlashDeal__Updat__7C6F7215");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__FlashDeal__Creat__6C390A4C");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.FlashDealsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__FlashDeal__Updat__6D2D2E85");
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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__GoodsRece__Batch__62CF9BA3");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__GoodsRece__Creat__65AC084E");

            entity.HasOne(d => d.GoodsReceipt).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.GoodsReceiptId)
                .HasConstraintName("FK__GoodsRece__Goods__5EFF0ABF");

            entity.HasOne(d => d.Product).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoodsRece__Produ__60E75331");

            entity.HasOne(d => d.PurchaseOrderLine).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.PurchaseOrderLineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoodsRece__Purch__5FF32EF8");

            entity.HasOne(d => d.Variant).WithMany(p => p.GoodsReceiptLines)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__GoodsRece__Varia__61DB776A");
        });

        modelBuilder.Entity<GoodsReceipts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Receipts");

            entity.HasIndex(e => e.ReceiptNumber, "UX_Receipts_No").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Condition).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReceiptNumber).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GoodsReceiptsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__GoodsRece__Creat__59463169");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoodsRece__Purch__5575A085");

            entity.HasOne(d => d.ReceivedByUser).WithMany(p => p.GoodsReceiptsReceivedByUser)
                .HasForeignKey(d => d.ReceivedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoodsRece__Recei__575DE8F7");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.GoodsReceiptsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__GoodsRece__Updat__5A3A55A2");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.GoodsReceipts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoodsRece__Wareh__5669C4BE");
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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__Inventory__Batch__5D4BCC77");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Inventory__Creat__60283922");

            entity.HasOne(d => d.InventoryAdjustment).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.InventoryAdjustmentId)
                .HasConstraintName("FK__Inventory__Inven__5A6F5FCC");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__5B638405");

            entity.HasOne(d => d.Variant).WithMany(p => p.InventoryAdjustmentLines)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__Inventory__Varia__5C57A83E");
        });

        modelBuilder.Entity<InventoryAdjustments>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Adjustments");

            entity.HasIndex(e => e.AdjustmentNo, "UX_InventoryAdjustments").IsUnique();

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
                .HasForeignKey(d => d.ApprovedByUserId)
                .HasConstraintName("FK__Inventory__Appro__52CE3E04");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InventoryAdjustmentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Inventory__Creat__54B68676");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InventoryAdjustmentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Inventory__Updat__55AAAAAF");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryAdjustments)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__4FF1D159");
        });

        modelBuilder.Entity<Invoices>(entity =>
        {
            entity.HasIndex(e => e.InvoiceNumber, "UX_Invoices_Number").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Invoices__Create__511AFFBC");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Invoices__OrderI__4F32B74A");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InvoicesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Invoices__Update__520F23F5");
        });

        modelBuilder.Entity<LoyaltyTransactions>(entity =>
        {
            entity.HasIndex(e => new { e.CustomerId, e.TransactionDate }, "IX_LoyaltyTransactions").IsDescending(false, true);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.TransactionType).HasMaxLength(20);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.LoyaltyTransactionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__LoyaltyTr__Creat__21A0F6C4");

            entity.HasOne(d => d.Customer).WithMany(p => p.LoyaltyTransactions)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__LoyaltyTr__Custo__1CDC41A7");

            entity.HasOne(d => d.Order).WithMany(p => p.LoyaltyTransactions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_LoyaltyTransactions_Orders");

            entity.HasOne(d => d.PosTrans).WithMany(p => p.LoyaltyTransactions)
                .HasForeignKey(d => d.PosTransId)
                .HasConstraintName("FK_LoyaltyTransactions_Sales");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.LoyaltyTransactionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__LoyaltyTr__Updat__22951AFD");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__MediaAsse__Creat__740F363E");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.MediaAssetsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__MediaAsse__Updat__75035A77");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.MediaAssetsUploadedByNavigation)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK__MediaAsse__Uploa__7226EDCC");
        });

        modelBuilder.Entity<Menus>(entity =>
        {
            entity.HasIndex(e => e.MenuCode, "UX_Menus_Code").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Menus__CreatedBy__245D67DE");

            entity.HasOne(d => d.ParentMenu).WithMany(p => p.InverseParentMenu)
                .HasForeignKey(d => d.ParentMenuId)
                .HasConstraintName("FK__Menus__ParentMen__1CBC4616");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.MenusUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Menus__UpdatedBy__25518C17");
        });

        modelBuilder.Entity<NewsletterSubscribers>(entity =>
        {
            entity.HasIndex(e => e.Email, "UX_Subscribers_Email").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SubscribedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.NewsletterSubscribers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Newslette__Custo__55CAA640");
        });

        modelBuilder.Entity<Notifications>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Notifications_User").HasFilter("([IsDeleted]=(0) AND [IsRead]=(0))");

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Notificat__Creat__6AC5C326");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationsUser)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Notificat__UserI__650CE9D0");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__OrderBund__Creat__4979DDF4");

            entity.HasOne(d => d.Group).WithMany(p => p.OrderBundleSelections)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderBund__Group__45A94D10");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderBundleSelections)
                .HasForeignKey(d => d.OrderItemId)
                .HasConstraintName("FK__OrderBund__Order__44B528D7");

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderBundleSelections)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderBund__Varia__469D7149");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__OrderItem__Creat__3FF073BA");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderItemTaxes)
                .HasForeignKey(d => d.OrderItemId)
                .HasConstraintName("FK__OrderItem__Order__3D14070F");

            entity.HasOne(d => d.TaxRateNavigation).WithMany(p => p.OrderItemTaxes)
                .HasForeignKey(d => d.TaxRateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__TaxRa__3E082B48");
        });

        modelBuilder.Entity<OrderItems>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_OrderItems_Order").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.ProductId, "IX_OrderItems_Product").HasFilter("([IsDeleted]=(0))");

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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__OrderItem__Batch__2FBA0BF1");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__OrderItem__Creat__375B2DB9");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__2BE97B0D");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__2CDD9F46");

            entity.HasOne(d => d.Seller).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK__OrderItem__Selle__2EC5E7B8");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__OrderItem__Updat__384F51F2");

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__OrderItem__Varia__2DD1C37F");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__OrderRetu__Creat__7E77B618");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderReturnItems)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderRetu__Order__79B300FB");

            entity.HasOne(d => d.Return).WithMany(p => p.OrderReturnItems)
                .HasForeignKey(d => d.ReturnId)
                .HasConstraintName("FK__OrderRetu__Retur__78BEDCC2");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrderReturnItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__OrderRetu__Updat__7F6BDA51");
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
            entity.HasIndex(e => new { e.CustomerId, e.OrderDate }, "IX_Orders_Customer")
                .IsDescending(false, true)
                .HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.StatusCode, "IX_Orders_Status").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.OrderNumber, "UX_Orders_Number").IsUnique();

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
                .HasForeignKey(d => d.AppliedDiscountId)
                .HasConstraintName("FK__Orders__AppliedD__19CACAD2");

            entity.HasOne(d => d.BillingAddress).WithMany(p => p.OrdersBillingAddress)
                .HasForeignKey(d => d.BillingAddressId)
                .HasConstraintName("FK__Orders__BillingA__18D6A699");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrdersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Orders__CreatedB__2354350C");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Customer__150615B5");

            entity.HasOne(d => d.ShippingAddress).WithMany(p => p.OrdersShippingAddress)
                .HasForeignKey(d => d.ShippingAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Shipping__17E28260");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__StatusCo__1BB31344");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.OrdersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Orders__UpdatedB__24485945");

            entity.HasOne(d => d.User).WithMany(p => p.OrdersUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserId__15FA39EE");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Orders)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__Orders__Warehous__16EE5E27");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PaymentGa__Creat__5AA469F6");

            entity.HasOne(d => d.MethodCodeNavigation).WithMany(p => p.PaymentGateways)
                .HasForeignKey(d => d.MethodCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentGa__Metho__56D3D912");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PaymentGatewaysUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PaymentGa__Updat__5B988E2F");
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
            entity.HasIndex(e => e.OrderId, "IX_Payments_Order").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.TransactionId, "UX_Payments_TransactionId")
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Payments__Create__67FE6514");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Curren__66161CA2");

            entity.HasOne(d => d.MethodCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.MethodCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Method__61516785");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Payments__OrderI__605D434C");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Status__6339AFF7");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PaymentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Payments__Update__68F2894D");
        });

        modelBuilder.Entity<Permissions>(entity =>
        {
            entity.HasIndex(e => e.PermissionCode, "UX_Permissions_Code").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Permissio__Creat__114A936A");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PermissionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Permissio__Updat__123EB7A3");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PickupPoi__Creat__17AD7836");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PickupPointsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PickupPoi__Updat__18A19C6F");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PickupPoints)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__PickupPoi__Wareh__14D10B8B");
        });

        modelBuilder.Entity<PosCounters>(entity =>
        {
            entity.HasIndex(e => new { e.WarehouseId, e.CounterCode }, "UX_PosCounters").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CounterCode).HasMaxLength(50);
            entity.Property(e => e.CounterName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosCountersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosCounte__Creat__638EB5B2");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosCountersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PosCounte__Updat__6482D9EB");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PosCounters)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosCounte__Wareh__60B24907");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosPaymen__Creat__3C3FDE67");

            entity.HasOne(d => d.MethodCodeNavigation).WithMany(p => p.PosPaymentTenders)
                .HasForeignKey(d => d.MethodCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosPaymen__Metho__396371BC");

            entity.HasOne(d => d.Transaction).WithMany(p => p.PosPaymentTenders)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK__PosPaymen__Trans__386F4D83");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosPaymentTendersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PosPaymen__Updat__3D3402A0");
        });

        modelBuilder.Entity<PosTerminals>(entity =>
        {
            entity.HasIndex(e => new { e.PosCounterId, e.TerminalCode }, "UX_PosTerminals").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosTermin__Creat__6D181FEC");

            entity.HasOne(d => d.PosCounter).WithMany(p => p.PosTerminals)
                .HasForeignKey(d => d.PosCounterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTermin__PosCo__6A3BB341");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosTerminalsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PosTermin__Updat__6E0C4425");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosTransa__Creat__46BD6CDA");

            entity.HasOne(d => d.Group).WithMany(p => p.PosTransactionBundleSelections)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__Group__42ECDBF6");

            entity.HasOne(d => d.PosTransactionLine).WithMany(p => p.PosTransactionBundleSelections)
                .HasForeignKey(d => d.PosTransactionLineId)
                .HasConstraintName("FK__PosTransa__PosTr__41F8B7BD");

            entity.HasOne(d => d.Variant).WithMany(p => p.PosTransactionBundleSelections)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__Varia__43E1002F");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosTransa__Creat__33AA9866");

            entity.HasOne(d => d.PosTransactionLine).WithMany(p => p.PosTransactionLineTaxes)
                .HasForeignKey(d => d.PosTransactionLineId)
                .HasConstraintName("FK__PosTransa__PosTr__30CE2BBB");

            entity.HasOne(d => d.TaxRateNavigation).WithMany(p => p.PosTransactionLineTaxes)
                .HasForeignKey(d => d.TaxRateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__TaxRa__31C24FF4");
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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__PosTransa__Batch__255C790F");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionLinesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosTransa__Creat__2B155265");

            entity.HasOne(d => d.Product).WithMany(p => p.PosTransactionLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__Produ__2374309D");

            entity.HasOne(d => d.Transaction).WithMany(p => p.PosTransactionLines)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK__PosTransa__Trans__22800C64");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosTransactionLinesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PosTransa__Updat__2C09769E");

            entity.HasOne(d => d.Variant).WithMany(p => p.PosTransactionLines)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__PosTransa__Varia__246854D6");
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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__PosTransa__Batch__1372D2FE");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosTransa__Creat__164F3FA9");

            entity.HasOne(d => d.PosTransactionReturn).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.PosTransactionReturnId)
                .HasConstraintName("FK__PosTransa__PosTr__10966653");

            entity.HasOne(d => d.Product).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__Produ__118A8A8C");

            entity.HasOne(d => d.Variant).WithMany(p => p.PosTransactionReturnLines)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__PosTransa__Varia__127EAEC5");
        });

        modelBuilder.Entity<PosTransactionReturns>(entity =>
        {
            entity.HasIndex(e => e.ReturnNo, "UX_PosReturns_No").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosTransa__Creat__0ADD8CFD");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PosTransactionReturnsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK__PosTransa__Creat__08F5448B");

            entity.HasOne(d => d.Customer).WithMany(p => p.PosTransactionReturns)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__PosTransa__Custo__070CFC19");

            entity.HasOne(d => d.Sale).WithMany(p => p.PosTransactionReturns)
                .HasForeignKey(d => d.SaleId)
                .HasConstraintName("FK_PosTransactionReturns_Sales");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosTransactionReturnsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PosTransa__Updat__0BD1B136");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PosTransactionReturns)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__Wareh__0618D7E0");
        });

        modelBuilder.Entity<PosTransactions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Sales");

            entity.HasIndex(e => new { e.CashierId, e.SaleDate }, "IX_Sales_Cashier")
                .IsDescending(false, true)
                .HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.CustomerId, "IX_Sales_Customer").HasFilter("([CustomerId] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.CashShiftId, "IX_Sales_Shift").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => new { e.WarehouseId, e.SaleDate }, "IX_Sales_Warehouse").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.ReceiptNumber, "UX_Sales_Receipt").IsUnique();

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
                .HasForeignKey(d => d.AppliedDiscountId)
                .HasConstraintName("FK__PosTransa__Appli__0D84EF7E");

            entity.HasOne(d => d.CashShift).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.CashShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__CashS__06D7F1EF");

            entity.HasOne(d => d.CashierEmployee).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.CashierEmployeeId)
                .HasConstraintName("FK__PosTransa__Cashi__0AA882D3");

            entity.HasOne(d => d.Cashier).WithMany(p => p.PosTransactionsCashier)
                .HasForeignKey(d => d.CashierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__Cashi__09B45E9A");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PosTransactionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PosTransa__Creat__19EAC663");

            entity.HasOne(d => d.Customer).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__PosTransa__Custo__0B9CA70C");

            entity.HasOne(d => d.PosCounter).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.PosCounterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__PosCo__07CC1628");

            entity.HasOne(d => d.PosTerminal).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.PosTerminalId)
                .HasConstraintName("FK__PosTransa__PosTe__08C03A61");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PosTransactionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PosTransa__Updat__1ADEEA9C");

            entity.HasOne(d => d.VoidedByNavigation).WithMany(p => p.PosTransactionsVoidedByNavigation)
                .HasForeignKey(d => d.VoidedBy)
                .HasConstraintName("FK__PosTransa__Voide__18027DF1");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PosTransactions)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PosTransa__Wareh__0C90CB45");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PriceList__Creat__63A3C44B");

            entity.HasOne(d => d.PriceList).WithMany(p => p.PriceListItems)
                .HasForeignKey(d => d.PriceListId)
                .HasConstraintName("FK__PriceList__Price__5EDF0F2E");

            entity.HasOne(d => d.Product).WithMany(p => p.PriceListItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__PriceList__Produ__5FD33367");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PriceListItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PriceList__Updat__6497E884");

            entity.HasOne(d => d.Variant).WithMany(p => p.PriceListItems)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__PriceList__Varia__60C757A0");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PriceList__Creat__592635D8");

            entity.HasOne(d => d.TierCodeNavigation).WithMany(p => p.PriceLists)
                .HasForeignKey(d => d.TierCode)
                .HasConstraintName("FK__PriceList__TierC__5649C92D");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PriceListsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PriceList__Updat__5A1A5A11");
        });

        modelBuilder.Entity<ProductAttributeLinks>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.AttributeTypeId }, "UX_ProductAttributeLinks").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsRequired).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.AttributeType).WithMany(p => p.ProductAttributeLinks)
                .HasForeignKey(d => d.AttributeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductAt__Attri__27C3E46E");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductAttributeLinks)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductAt__Creat__2B947552");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductAttributeLinks)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductAt__Produ__26CFC035");
        });

        modelBuilder.Entity<ProductBatches>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.BatchNo }, "UX_ProductBatches").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.BatchNo).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductBatchesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductBa__Creat__13BCEBC1");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductBatches)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductBa__Produ__0FEC5ADD");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductBatchesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ProductBa__Updat__14B10FFA");
        });

        modelBuilder.Entity<ProductCollectionItems>(entity =>
        {
            entity.HasIndex(e => new { e.ProductCollectionId, e.ProductId }, "UX_ProductCollectionItems").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductCollectionItems)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductCo__Creat__4F9CCB9E");

            entity.HasOne(d => d.ProductCollection).WithMany(p => p.ProductCollectionItems)
                .HasForeignKey(d => d.ProductCollectionId)
                .HasConstraintName("FK__ProductCo__Produ__4BCC3ABA");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCollectionItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductCo__Produ__4CC05EF3");
        });

        modelBuilder.Entity<ProductCollections>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Collections");

            entity.HasIndex(e => e.Slug, "UX_Collections_Slug").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(150);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductCollectionsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductCo__Creat__451F3D2B");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductCollectionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ProductCo__Updat__46136164");
        });

        modelBuilder.Entity<ProductConditions>(entity =>
        {
            entity.HasKey(e => e.ConditionCode);

            entity.Property(e => e.ConditionCode).HasMaxLength(20);
            entity.Property(e => e.DisplayName).HasMaxLength(50);
        });

        modelBuilder.Entity<ProductImages>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_ProductImages_Product").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AltText).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductImagesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductIm__Creat__58671BC9");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductIm__Produ__53A266AC");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductImagesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ProductIm__Updat__595B4002");

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__ProductIm__Varia__54968AE5");
        });

        modelBuilder.Entity<ProductMedia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Media");

            entity.HasIndex(e => new { e.ProductId, e.Scope, e.SortOrder }, "IX_Media").HasFilter("([IsDeleted]=(0))");

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductMe__Creat__04459E07");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductMedia)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductMe__Produ__79C80F94");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductMediaUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ProductMe__Updat__0539C240");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.ProductMediaUploadedByNavigation)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK__ProductMe__Uploa__025D5595");

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductMedia)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__ProductMe__Varia__7ABC33CD");
        });

        modelBuilder.Entity<ProductMediaBlob>(entity =>
        {
            entity.HasKey(e => e.MediaId);

            entity.Property(e => e.MediaId).ValueGeneratedNever();

            entity.HasOne(d => d.Media).WithOne(p => p.ProductMediaBlob)
                .HasForeignKey<ProductMediaBlob>(d => d.MediaId)
                .HasConstraintName("FK_ProductMediaBlob");
        });

        modelBuilder.Entity<ProductPriceHistories>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.EffectiveFrom }, "IX_ProductPriceHistories").IsDescending(false, true);

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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductPr__Chang__075714DC");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductPriceHistoriesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductPr__Creat__0A338187");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPriceHistories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductPr__Produ__0662F0A3");
        });

        modelBuilder.Entity<ProductSpecificationValues>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductSpecificationValuesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductSp__Creat__4DE98D56");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSpecificationValues)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductSp__Produ__4A18FC72");

            entity.HasOne(d => d.Spec).WithMany(p => p.ProductSpecificationValues)
                .HasForeignKey(d => d.SpecId)
                .HasConstraintName("FK__ProductSp__SpecI__4C0144E4");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductSpecificationValuesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ProductSp__Updat__4EDDB18F");

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductSpecificationValues)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__ProductSp__Varia__4B0D20AB");
        });

        modelBuilder.Entity<ProductSpecifications>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Specs");

            entity.HasIndex(e => e.SpecName, "UX_ProductSpecifications").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SpecName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductSpecifications)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductSp__Creat__45544755");
        });

        modelBuilder.Entity<ProductSupplierLinks>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductSuppliers");

            entity.HasIndex(e => new { e.ProductId, e.SupplierId }, "UX_ProductSupplierLinks").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductSu__Creat__00AA174D");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSupplierLinks)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductSu__Produ__7BE56230");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ProductSupplierLinks)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__ProductSu__Suppl__7CD98669");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductSupplierLinksUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ProductSu__Updat__019E3B86");
        });

        modelBuilder.Entity<ProductTags>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.TagId });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductTa__Creat__6A85CC04");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductTa__Produ__67A95F59");

            entity.HasOne(d => d.Tag).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK__ProductTa__TagId__689D8392");
        });

        modelBuilder.Entity<ProductTaxRates>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductTax");

            entity.HasIndex(e => new { e.ProductId, e.TaxRateId }, "UX_ProductTaxes").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductTaxRatesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductTa__Creat__62E4AA3C");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTaxRates)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductTa__Produ__5F141958");

            entity.HasOne(d => d.TaxRate).WithMany(p => p.ProductTaxRates)
                .HasForeignKey(d => d.TaxRateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductTa__TaxRa__60083D91");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductTaxRatesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ProductTa__Updat__63D8CE75");
        });

        modelBuilder.Entity<ProductVariants>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_Variants_Product").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Barcode, "UX_ProductVariants_Barcode")
                .IsUnique()
                .HasFilter("([Barcode] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.Sku, "UX_ProductVariants_SKU")
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ProductVa__Creat__2022C2A6");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductVa__Produ__1975C517");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductVariantsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ProductVa__Updat__2116E6DF");
        });

        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasIndex(e => new { e.IsActive, e.ProductType }, "IX_Products_Active").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.BrandId, "IX_Products_Brand").HasFilter("([BrandId] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.CategoryId, "IX_Products_Category").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.Barcode, "UX_Products_Barcode")
                .IsUnique()
                .HasFilter("([Barcode] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.Sku, "UX_Products_SKU")
                .IsUnique()
                .HasFilter("([SKU] IS NOT NULL AND [IsDeleted]=(0))");

            entity.HasIndex(e => e.Slug, "UX_Products_Slug").IsUnique();

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
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Products__BrandI__61316BF4");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__603D47BB");

            entity.HasOne(d => d.Color).WithMany(p => p.Products)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("FK__Products__ColorI__6225902D");

            entity.HasOne(d => d.ConditionCodeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ConditionCode)
                .HasConstraintName("FK__Products__Condit__6501FCD8");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Products__Create__753864A1");

            entity.HasOne(d => d.Seller).WithMany(p => p.Products)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Products_Sellers");

            entity.HasOne(d => d.TaxRate).WithMany(p => p.Products)
                .HasForeignKey(d => d.TaxRateId)
                .HasConstraintName("FK__Products__TaxRat__640DD89F");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK__Products__UnitId__6319B466");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Products__Update__762C88DA");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PurchaseO__Creat__33208881");

            entity.HasOne(d => d.PurchaseOrderLine).WithMany(p => p.PurchaseOrderLineTaxes)
                .HasForeignKey(d => d.PurchaseOrderLineId)
                .HasConstraintName("FK__PurchaseO__Purch__30441BD6");

            entity.HasOne(d => d.TaxRateNavigation).WithMany(p => p.PurchaseOrderLineTaxes)
                .HasForeignKey(d => d.TaxRateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__TaxRa__3138400F");
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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__PurchaseO__Batch__23DE44F1");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseOrderLinesCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PurchaseO__Creat__2A8B4280");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Produ__21F5FC7F");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.PurchaseOrderId)
                .HasConstraintName("FK__PurchaseO__Purch__2101D846");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PurchaseOrderLinesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PurchaseO__Updat__2B7F66B9");

            entity.HasOne(d => d.Variant).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__PurchaseO__Varia__22EA20B8");
        });

        modelBuilder.Entity<PurchaseOrders>(entity =>
        {
            entity.HasIndex(e => e.OrderNumber, "UX_PurchaseOrders_No").IsUnique();

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
                .HasForeignKey(d => d.ApprovedByUserId)
                .HasConstraintName("FK__PurchaseO__Appro__0CFADF99");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseOrdersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PurchaseO__Creat__1B48FEF0");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PurchaseOrdersCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Creat__0C06BB60");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Suppl__0A1E72EE");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PurchaseOrdersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PurchaseO__Updat__1C3D2329");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__PurchaseO__Wareh__0B129727");
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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__PurchaseR__Batch__4AF81212");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PurchaseR__Creat__4FBCC72F");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseR__Produ__490FC9A0");

            entity.HasOne(d => d.PurchaseReturn).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.PurchaseReturnId)
                .HasConstraintName("FK__PurchaseR__Purch__481BA567");

            entity.HasOne(d => d.Variant).WithMany(p => p.PurchaseReturnLines)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__PurchaseR__Varia__4A03EDD9");
        });

        modelBuilder.Entity<PurchaseReturns>(entity =>
        {
            entity.HasIndex(e => e.ReturnNo, "UX_PurchReturns_No").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PurchaseR__Creat__4262CC11");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PurchaseReturnsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK__PurchaseR__Creat__407A839F");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseR__Purch__38D961D7");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseR__Suppl__3AC1AA49");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PurchaseReturnsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PurchaseR__Updat__4356F04A");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseR__Wareh__3BB5CE82");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__QuoteItem__Creat__0E591826");

            entity.HasOne(d => d.Product).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__QuoteItem__Produ__07AC1A97");

            entity.HasOne(d => d.Quote).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.QuoteId)
                .HasConstraintName("FK__QuoteItem__Quote__06B7F65E");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.QuoteItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__QuoteItem__Updat__0F4D3C5F");

            entity.HasOne(d => d.Variant).WithMany(p => p.QuoteItems)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__QuoteItem__Varia__08A03ED0");
        });

        modelBuilder.Entity<Quotes>(entity =>
        {
            entity.HasIndex(e => e.QuoteNo, "UX_Quotes_No").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Quotes__CreatedB__00FF1D08");

            entity.HasOne(d => d.Customer).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Quotes__Customer__7775B2CE");

            entity.HasOne(d => d.Order).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Quotes_Orders");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.QuotesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Quotes__UpdatedB__01F34141");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Quotes__Warehous__7869D707");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__RefundReq__Creat__21C0F255");

            entity.HasOne(d => d.Customer).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__RefundReq__Custo__1C0818FF");

            entity.HasOne(d => d.Order).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__RefundReq__Order__1B13F4C6");

            entity.HasOne(d => d.Return).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.ReturnId)
                .HasConstraintName("FK__RefundReq__Retur__1CFC3D38");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefundReq__Statu__1EE485AA");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RefundRequestsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__RefundReq__Updat__22B5168E");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ReorderRu__Creat__0371755F");

            entity.HasOne(d => d.NotifyUser).WithMany(p => p.ReorderRulesNotifyUser)
                .HasForeignKey(d => d.NotifyUserId)
                .HasConstraintName("FK__ReorderRu__Notif__009508B4");

            entity.HasOne(d => d.PreferredSupplier).WithMany(p => p.ReorderRules)
                .HasForeignKey(d => d.PreferredSupplierId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__ReorderRu__Prefe__7FA0E47B");

            entity.HasOne(d => d.Product).WithMany(p => p.ReorderRules)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReorderRu__Produ__7CC477D0");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ReorderRulesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ReorderRu__Updat__04659998");

            entity.HasOne(d => d.Variant).WithMany(p => p.ReorderRules)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__ReorderRu__Varia__7DB89C09");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.ReorderRules)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__ReorderRu__Wareh__7EACC042");
        });

        modelBuilder.Entity<ReturnStatuses>(entity =>
        {
            entity.HasKey(e => e.StatusCode);

            entity.Property(e => e.StatusCode).HasMaxLength(30);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
        });

        modelBuilder.Entity<Returns>(entity =>
        {
            entity.HasIndex(e => e.ReturnNumber, "UX_Returns_Number").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Returns__Created__7306036C");

            entity.HasOne(d => d.Order).WithMany(p => p.Returns)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Returns__OrderId__6B64E1A4");

            entity.HasOne(d => d.ProcessedByUser).WithMany(p => p.ReturnsProcessedByUser)
                .HasForeignKey(d => d.ProcessedByUserId)
                .HasConstraintName("FK__Returns__Process__6C5905DD");

            entity.HasOne(d => d.RefundMethodCodeNavigation).WithMany(p => p.Returns)
                .HasForeignKey(d => d.RefundMethodCode)
                .HasConstraintName("FK__Returns__RefundM__711DBAFA");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Returns)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Returns__StatusC__6F357288");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ReturnsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Returns__Updated__73FA27A5");
        });

        modelBuilder.Entity<ReviewHelpfulness>(entity =>
        {
            entity.HasKey(e => new { e.ReviewId, e.UserId });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewHelpfulness)
                .HasForeignKey(d => d.ReviewId)
                .HasConstraintName("FK__ReviewHel__Revie__36BC0F3B");

            entity.HasOne(d => d.User).WithMany(p => p.ReviewHelpfulness)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ReviewHel__UserI__37B03374");
        });

        modelBuilder.Entity<Reviews>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_Reviews_Product").HasFilter("([IsDeleted]=(0) AND [IsApproved]=(1))");

            entity.HasIndex(e => new { e.CustomerId, e.ProductId }, "UX_Reviews_CustomerProduct").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ReviewsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Reviews__Created__31F75A1E");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Custome__2962141D");

            entity.HasOne(d => d.Order).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Reviews__OrderId__2A563856");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Product__286DEFE4");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ReviewsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Reviews__Updated__32EB7E57");
        });

        modelBuilder.Entity<RoleClaims>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__RoleClaim__RoleI__01142BA1");
        });

        modelBuilder.Entity<RoleMenus>(entity =>
        {
            entity.HasIndex(e => new { e.RoleId, e.MenuId }, "UX_RoleMenus").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CanView).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoleMenusCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__RoleMenus__Creat__32AB8735");

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK__RoleMenus__MenuI__2BFE89A6");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__RoleMenus__RoleI__2B0A656D");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RoleMenusUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__RoleMenus__Updat__339FAB6E");
        });

        modelBuilder.Entity<RolePermissions>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });

            entity.Property(e => e.IsGranted).HasDefaultValue(true);

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK__RolePermi__Permi__17036CC0");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__RolePermi__RoleI__160F4887");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "UX_Roles_Name")
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
            entity.HasIndex(e => e.Keyword, "UX_Searches_Keyword").IsUnique();

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
            entity.HasIndex(e => e.Slug, "UX_Sellers_Slug").IsUnique();

            entity.HasIndex(e => e.UserId, "UX_Sellers_UserId").IsUnique();

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
                .HasForeignKey(d => d.ApprovedByUserId)
                .HasConstraintName("FK__Sellers__Approve__2E06CDA9");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SellersCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Sellers__Created__30E33A54");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SellersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Sellers__Updated__31D75E8D");

            entity.HasOne(d => d.User).WithOne(p => p.SellersUser)
                .HasForeignKey<Sellers>(d => d.UserId)
                .HasConstraintName("FK__Sellers__UserId__2942188C");
        });

        modelBuilder.Entity<ShipmentStatuses>(entity =>
        {
            entity.HasKey(e => e.StatusCode);

            entity.Property(e => e.StatusCode).HasMaxLength(30);
            entity.Property(e => e.DisplayName).HasMaxLength(80);
        });

        modelBuilder.Entity<Shipments>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_Shipments_Order").HasFilter("([IsDeleted]=(0))");

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
                .HasForeignKey(d => d.CarrierId)
                .HasConstraintName("FK__Shipments__Carri__1F4E99FE");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ShipmentsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Shipments__Creat__25FB978D");

            entity.HasOne(d => d.Order).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Shipments__Order__1D66518C");

            entity.HasOne(d => d.ShippingMethod).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.ShippingMethodId)
                .HasConstraintName("FK__Shipments__Shipp__1E5A75C5");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.StatusCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shipments__Statu__222B06A9");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ShipmentsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Shipments__Updat__26EFBBC6");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__Shipments__Wareh__2042BE37");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ShippingC__Creat__7DEDA633");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ShippingCarriersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ShippingC__Updat__7EE1CA6C");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ShippingM__Creat__75586032");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ShippingMethodsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ShippingM__Updat__764C846B");
        });

        modelBuilder.Entity<StaticPages>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Pages");

            entity.HasIndex(e => e.Slug, "UX_Pages_Slug").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__StaticPag__Creat__33758E3C");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.StaticPagesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__StaticPag__Updat__3469B275");
        });

        modelBuilder.Entity<StockItems>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Stock");

            entity.HasIndex(e => new { e.ProductId, e.WarehouseId }, "IX_Stock_Product").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.WarehouseId, "IX_Stock_Warehouse").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => new { e.ProductId, e.VariantId, e.BatchId, e.WarehouseId }, "UX_Stock").IsUnique();

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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__StockItem__Batch__2E90DD8E");

            entity.HasOne(d => d.CountedByUser).WithMany(p => p.StockItemsCountedByUser)
                .HasForeignKey(d => d.CountedByUserId)
                .HasConstraintName("FK__StockItem__Count__353DDB1D");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__StockItem__Creat__381A47C8");

            entity.HasOne(d => d.Product).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockItem__Produ__2CA8951C");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.StockItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__StockItem__Updat__390E6C01");

            entity.HasOne(d => d.Variant).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__StockItem__Varia__2D9CB955");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.StockItems)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockItem__Wareh__2F8501C7");
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
            entity.HasIndex(e => new { e.ProductId, e.OccurredAt }, "IX_StockMovements")
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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__StockMove__Batch__40AF8DC9");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__StockMove__Creat__4944D3CA");

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.StockMovementsFromWarehouse)
                .HasForeignKey(d => d.FromWarehouseId)
                .HasConstraintName("FK__StockMove__FromW__4297D63B");

            entity.HasOne(d => d.MovementTypeCodeNavigation).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.MovementTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockMove__Movem__44801EAD");

            entity.HasOne(d => d.Product).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockMove__Produ__3EC74557");

            entity.HasOne(d => d.StockItem).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.StockItemId)
                .HasConstraintName("FK__StockMove__Stock__41A3B202");

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.StockMovementsToWarehouse)
                .HasForeignKey(d => d.ToWarehouseId)
                .HasConstraintName("FK__StockMove__ToWar__438BFA74");

            entity.HasOne(d => d.Variant).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__StockMove__Varia__3FBB6990");
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
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__StockTran__Batch__75235608");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__StockTran__Creat__77FFC2B3");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTran__Produ__733B0D96");

            entity.HasOne(d => d.Transfer).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.TransferId)
                .HasConstraintName("FK__StockTran__Trans__7246E95D");

            entity.HasOne(d => d.Variant).WithMany(p => p.StockTransferLines)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__StockTran__Varia__742F31CF");
        });

        modelBuilder.Entity<StockTransfers>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Transfers");

            entity.HasIndex(e => e.TransferNo, "UX_Transfers_No").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__StockTran__Creat__6C8E1007");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.StockTransfersCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK__StockTran__Creat__6AA5C795");

            entity.HasOne(d => d.FromWarehouse).WithMany(p => p.StockTransfersFromWarehouse)
                .HasForeignKey(d => d.FromWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTran__FromW__65E11278");

            entity.HasOne(d => d.ToWarehouse).WithMany(p => p.StockTransfersToWarehouse)
                .HasForeignKey(d => d.ToWarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTran__ToWar__66D536B1");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.StockTransfersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__StockTran__Updat__6D823440");
        });

        modelBuilder.Entity<Suppliers>(entity =>
        {
            entity.HasIndex(e => e.SupplierCode, "UX_Suppliers_Code").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Suppliers__Creat__7EF6D905");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SuppliersUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Suppliers__Updat__7FEAFD3E");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SupportTi__Creat__0B3292B8");

            entity.HasOne(d => d.Sender).WithMany(p => p.SupportTicketMessagesSender)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__SupportTi__Sende__0856260D");

            entity.HasOne(d => d.SupportTicket).WithMany(p => p.SupportTicketMessages)
                .HasForeignKey(d => d.SupportTicketId)
                .HasConstraintName("FK__SupportTi__Suppo__076201D4");
        });

        modelBuilder.Entity<SupportTickets>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tickets");

            entity.HasIndex(e => e.TicketNumber, "UX_Tickets_No").IsUnique();

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
                .HasForeignKey(d => d.AssignedToId)
                .HasConstraintName("FK__SupportTi__Assig__7AFC2AEF");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SupportTicketsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SupportTi__Creat__01A9287E");

            entity.HasOne(d => d.Order).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__SupportTi__Order__7BF04F28");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SupportTicketsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__SupportTi__Updat__029D4CB7");

            entity.HasOne(d => d.User).WithMany(p => p.SupportTicketsUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__SupportTi__UserI__7A0806B6");
        });

        modelBuilder.Entity<Tags>(entity =>
        {
            entity.HasIndex(e => e.Slug, "UX_Tags_Slug").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Slug).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TagsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Tags__CreatedBy__18B6AB08");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TagsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Tags__UpdatedBy__19AACF41");
        });

        modelBuilder.Entity<TaxRates>(entity =>
        {
            entity.HasIndex(e => e.TaxCode, "UX_TaxRates_Code").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__TaxRates__Create__5224328E");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TaxRatesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__TaxRates__Update__531856C7");
        });

        modelBuilder.Entity<Units>(entity =>
        {
            entity.HasIndex(e => e.Name, "UX_Units_Name").IsUnique();

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
                .HasForeignKey(d => d.BaseUnitId)
                .HasConstraintName("FK__Units__BaseUnitI__0E391C95");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UnitsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Units__CreatedBy__11158940");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.UnitsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Units__UpdatedBy__1209AD79");
        });

        modelBuilder.Entity<UserClaims>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.User).WithMany(p => p.UserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserClaim__UserI__7D439ABD");
        });

        modelBuilder.Entity<UserLogins>(entity =>
        {
            entity.HasIndex(e => new { e.LoginProvider, e.ProviderKey }, "UX_UserLogins_Provider").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserLogin__UserI__05D8E0BE");
        });

        modelBuilder.Entity<UserRefreshTokens>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_RefreshTokens_User").HasFilter("([IsDeleted]=(0))");

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__UserRefre__Creat__395884C4");

            entity.HasOne(d => d.User).WithMany(p => p.UserRefreshTokensUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserRefre__UserI__37703C52");
        });

        modelBuilder.Entity<UserTokens>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.LoginProvider, e.Name }, "UX_UserTokens").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserToken__UserI__0A9D95DB");
        });

        modelBuilder.Entity<UserRoles>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserRoles__UserI__787EE5A0");

            entity.HasOne(d => d.Role)
                .WithMany()
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__UserRoles__RoleI__797309D9");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "UX_Users_Email")
                .IsUnique()
                .HasFilter("([NormalizedEmail] IS NOT NULL)");

            entity.HasIndex(e => e.NormalizedUserName, "UX_Users_UserName")
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
            entity.HasIndex(e => new { e.ProductId, e.VariantId }, "UX_VariantAttributeMatrix").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.VariantAttributeMatrix)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__VariantAt__Creat__3DB3258D");

            entity.HasOne(d => d.Product).WithMany(p => p.VariantAttributeMatrix)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VariantAt__Produ__39E294A9");

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantAttributeMatrix)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__VariantAt__Varia__3AD6B8E2");
        });

        modelBuilder.Entity<VariantAttributeOptions>(entity =>
        {
            entity.HasIndex(e => new { e.VariantId, e.OptionId }, "UX_VariantAttributeOptions").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.VariantAttributeOptions)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__VariantAt__Creat__3429BB53");

            entity.HasOne(d => d.Option).WithMany(p => p.VariantAttributeOptions)
                .HasForeignKey(d => d.OptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VariantAt__Optio__324172E1");

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantAttributeOptions)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__VariantAt__Varia__314D4EA8");
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
            entity.HasIndex(e => new { e.WalletId, e.CreatedAt }, "IX_WalletTransactions").IsDescending(false, true);

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__WalletTra__Creat__5708E33C");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WalletTransactionsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__WalletTra__Updat__57FD0775");

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("FK__WalletTra__Walle__52442E1F");
        });

        modelBuilder.Entity<Warehouses>(entity =>
        {
            entity.HasIndex(e => e.Code, "UX_Warehouses_Code").IsUnique();

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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Warehouse__Creat__5F7E2DAC");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Warehouse__Paren__5AB9788F");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WarehousesUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Warehouse__Updat__607251E5");
        });

        modelBuilder.Entity<WishlistItems>(entity =>
        {
            entity.HasIndex(e => new { e.WishlistId, e.ProductId, e.VariantId }, "UX_WishlistItems").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WishlistItemsCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__WishlistI__Creat__4F87BD05");

            entity.HasOne(d => d.Product).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WishlistI__Produ__4AC307E8");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WishlistItemsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__WishlistI__Updat__507BE13E");

            entity.HasOne(d => d.Variant).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK__WishlistI__Varia__4BB72C21");

            entity.HasOne(d => d.Wishlist).WithMany(p => p.WishlistItems)
                .HasForeignKey(d => d.WishlistId)
                .HasConstraintName("FK__WishlistI__Wishl__49CEE3AF");
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
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Wishlists__Creat__4321E620");

            entity.HasOne(d => d.Customer).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Wishlists__Custo__3C74E891");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.WishlistsUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Wishlists__Updat__44160A59");

            entity.HasOne(d => d.User).WithMany(p => p.WishlistsUser)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Wishlists__UserI__3D690CCA");

            entity.HasOne(d => d.WishlistTypeCodeNavigation).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.WishlistTypeCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__Wishl__3F51553C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
