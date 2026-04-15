using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Users
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string? NormalizedUserName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; }

    public string PreferredLanguage { get; set; } = "en";

    public string TimeZone { get; set; } = "UTC";

    public DateTime? LastLoginAt { get; set; }

    public DateTime? LastPasswordChangedAt { get; set; }

    public string? EmailVerificationToken { get; set; }

    public DateTime? EmailVerificationExpiry { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetExpiry { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ActivityLogs> ActivityLogs { get; set; } = new List<ActivityLogs>();

    public virtual ICollection<AppSettings> AppSettingsCreatedByNavigation { get; set; } = new List<AppSettings>();

    public virtual ICollection<AppSettings> AppSettingsUpdatedByNavigation { get; set; } = new List<AppSettings>();

    public virtual ICollection<AttributeOptionMedia> AttributeOptionMediaCreatedByNavigation { get; set; } = new List<AttributeOptionMedia>();

    public virtual ICollection<AttributeOptionMedia> AttributeOptionMediaUpdatedByNavigation { get; set; } = new List<AttributeOptionMedia>();

    public virtual ICollection<AttributeOptions> AttributeOptionsCreatedByNavigation { get; set; } = new List<AttributeOptions>();

    public virtual ICollection<AttributeOptions> AttributeOptionsUpdatedByNavigation { get; set; } = new List<AttributeOptions>();

    public virtual ICollection<AttributeTypes> AttributeTypesCreatedByNavigation { get; set; } = new List<AttributeTypes>();

    public virtual ICollection<AttributeTypes> AttributeTypesUpdatedByNavigation { get; set; } = new List<AttributeTypes>();

    public virtual ICollection<AuditLogs> AuditLogs { get; set; } = new List<AuditLogs>();

    public virtual ICollection<BlogCategories> BlogCategoriesCreatedByNavigation { get; set; } = new List<BlogCategories>();

    public virtual ICollection<BlogCategories> BlogCategoriesUpdatedByNavigation { get; set; } = new List<BlogCategories>();

    public virtual ICollection<BlogComments> BlogCommentsCreatedByNavigation { get; set; } = new List<BlogComments>();

    public virtual ICollection<BlogComments> BlogCommentsUpdatedByNavigation { get; set; } = new List<BlogComments>();

    public virtual ICollection<BlogComments> BlogCommentsUser { get; set; } = new List<BlogComments>();

    public virtual ICollection<BlogTags> BlogTags { get; set; } = new List<BlogTags>();

    public virtual ICollection<Blogs> BlogsAuthor { get; set; } = new List<Blogs>();

    public virtual ICollection<Blogs> BlogsCreatedByNavigation { get; set; } = new List<Blogs>();

    public virtual ICollection<Blogs> BlogsUpdatedByNavigation { get; set; } = new List<Blogs>();

    public virtual ICollection<Brands> BrandsCreatedByNavigation { get; set; } = new List<Brands>();

    public virtual ICollection<Brands> BrandsUpdatedByNavigation { get; set; } = new List<Brands>();

    public virtual ICollection<BundleComponents> BundleComponentsCreatedByNavigation { get; set; } = new List<BundleComponents>();

    public virtual ICollection<BundleComponents> BundleComponentsUpdatedByNavigation { get; set; } = new List<BundleComponents>();

    public virtual ICollection<BundleOptionGroups> BundleOptionGroupsCreatedByNavigation { get; set; } = new List<BundleOptionGroups>();

    public virtual ICollection<BundleOptionGroups> BundleOptionGroupsUpdatedByNavigation { get; set; } = new List<BundleOptionGroups>();

    public virtual ICollection<BundleOptionItems> BundleOptionItemsCreatedByNavigation { get; set; } = new List<BundleOptionItems>();

    public virtual ICollection<BundleOptionItems> BundleOptionItemsUpdatedByNavigation { get; set; } = new List<BundleOptionItems>();

    public virtual ICollection<CartItems> CartItemsCreatedByNavigation { get; set; } = new List<CartItems>();

    public virtual ICollection<CartItems> CartItemsUpdatedByNavigation { get; set; } = new List<CartItems>();

    public virtual ICollection<Carts> CartsCreatedByNavigation { get; set; } = new List<Carts>();

    public virtual ICollection<Carts> CartsUpdatedByNavigation { get; set; } = new List<Carts>();

    public virtual ICollection<Carts> CartsUser { get; set; } = new List<Carts>();

    public virtual ICollection<CashDrawerEvents> CashDrawerEventsCreatedByNavigation { get; set; } = new List<CashDrawerEvents>();

    public virtual ICollection<CashDrawerEvents> CashDrawerEventsPerformedByNavigation { get; set; } = new List<CashDrawerEvents>();

    public virtual ICollection<CashShifts> CashShiftsClosedByUser { get; set; } = new List<CashShifts>();

    public virtual ICollection<CashShifts> CashShiftsCreatedByNavigation { get; set; } = new List<CashShifts>();

    public virtual ICollection<CashShifts> CashShiftsOpenedByUser { get; set; } = new List<CashShifts>();

    public virtual ICollection<CashShifts> CashShiftsUpdatedByNavigation { get; set; } = new List<CashShifts>();

    public virtual ICollection<Categories> CategoriesCreatedByNavigation { get; set; } = new List<Categories>();

    public virtual ICollection<Categories> CategoriesUpdatedByNavigation { get; set; } = new List<Categories>();

    public virtual ICollection<Colors> ColorsCreatedByNavigation { get; set; } = new List<Colors>();

    public virtual ICollection<Colors> ColorsUpdatedByNavigation { get; set; } = new List<Colors>();

    public virtual ICollection<ContactMessages> ContactMessages { get; set; } = new List<ContactMessages>();

    public virtual ICollection<Currencies> CurrenciesCreatedByNavigation { get; set; } = new List<Currencies>();

    public virtual ICollection<Currencies> CurrenciesUpdatedByNavigation { get; set; } = new List<Currencies>();

    public virtual ICollection<CustomerAddresses> CustomerAddressesCreatedByNavigation { get; set; } = new List<CustomerAddresses>();

    public virtual ICollection<CustomerAddresses> CustomerAddressesUpdatedByNavigation { get; set; } = new List<CustomerAddresses>();

    public virtual ICollection<CustomerAddresses> CustomerAddressesUser { get; set; } = new List<CustomerAddresses>();

    public virtual ICollection<CustomerProfiles> CustomerProfilesCreatedByNavigation { get; set; } = new List<CustomerProfiles>();

    public virtual ICollection<CustomerProfiles> CustomerProfilesUpdatedByNavigation { get; set; } = new List<CustomerProfiles>();

    public virtual ICollection<CustomerWallets> CustomerWalletsCreatedByNavigation { get; set; } = new List<CustomerWallets>();

    public virtual ICollection<CustomerWallets> CustomerWalletsUpdatedByNavigation { get; set; } = new List<CustomerWallets>();

    public virtual ICollection<Customers> CustomersCreatedByNavigation { get; set; } = new List<Customers>();

    public virtual ICollection<Customers> CustomersUpdatedByNavigation { get; set; } = new List<Customers>();

    public virtual Customers? CustomersUser { get; set; }

    public virtual ICollection<DayEndSummaries> DayEndSummariesClosedByUser { get; set; } = new List<DayEndSummaries>();

    public virtual ICollection<DayEndSummaries> DayEndSummariesCreatedByNavigation { get; set; } = new List<DayEndSummaries>();

    public virtual ICollection<DayEndSummaries> DayEndSummariesUpdatedByNavigation { get; set; } = new List<DayEndSummaries>();

    public virtual ICollection<DeliveryZoneRegions> DeliveryZoneRegionsCreatedByNavigation { get; set; } = new List<DeliveryZoneRegions>();

    public virtual ICollection<DeliveryZoneRegions> DeliveryZoneRegionsUpdatedByNavigation { get; set; } = new List<DeliveryZoneRegions>();

    public virtual ICollection<DeliveryZones> DeliveryZonesCreatedByNavigation { get; set; } = new List<DeliveryZones>();

    public virtual ICollection<DeliveryZones> DeliveryZonesUpdatedByNavigation { get; set; } = new List<DeliveryZones>();

    public virtual ICollection<DiscountUsageLog> DiscountUsageLogCreatedByNavigation { get; set; } = new List<DiscountUsageLog>();

    public virtual ICollection<DiscountUsageLog> DiscountUsageLogUser { get; set; } = new List<DiscountUsageLog>();

    public virtual ICollection<Discounts> DiscountsCreatedByNavigation { get; set; } = new List<Discounts>();

    public virtual ICollection<Discounts> DiscountsUpdatedByNavigation { get; set; } = new List<Discounts>();

    public virtual ICollection<EmailTemplates> EmailTemplatesCreatedByNavigation { get; set; } = new List<EmailTemplates>();

    public virtual ICollection<EmailTemplates> EmailTemplatesUpdatedByNavigation { get; set; } = new List<EmailTemplates>();

    public virtual ICollection<Employees> EmployeesCreatedByNavigation { get; set; } = new List<Employees>();

    public virtual ICollection<Employees> EmployeesUpdatedByNavigation { get; set; } = new List<Employees>();

    public virtual ICollection<Employees> EmployeesUser { get; set; } = new List<Employees>();

    public virtual ICollection<ExpenseCategories> ExpenseCategoriesCreatedByNavigation { get; set; } = new List<ExpenseCategories>();

    public virtual ICollection<ExpenseCategories> ExpenseCategoriesUpdatedByNavigation { get; set; } = new List<ExpenseCategories>();

    public virtual ICollection<Expenses> ExpensesCreatedByNavigation { get; set; } = new List<Expenses>();

    public virtual ICollection<Expenses> ExpensesCreatedByUser { get; set; } = new List<Expenses>();

    public virtual ICollection<Expenses> ExpensesUpdatedByNavigation { get; set; } = new List<Expenses>();

    public virtual ICollection<FlashDealProducts> FlashDealProductsCreatedByNavigation { get; set; } = new List<FlashDealProducts>();

    public virtual ICollection<FlashDealProducts> FlashDealProductsUpdatedByNavigation { get; set; } = new List<FlashDealProducts>();

    public virtual ICollection<FlashDeals> FlashDealsCreatedByNavigation { get; set; } = new List<FlashDeals>();

    public virtual ICollection<FlashDeals> FlashDealsUpdatedByNavigation { get; set; } = new List<FlashDeals>();

    public virtual ICollection<GoodsReceiptLines> GoodsReceiptLines { get; set; } = new List<GoodsReceiptLines>();

    public virtual ICollection<GoodsReceipts> GoodsReceiptsCreatedByNavigation { get; set; } = new List<GoodsReceipts>();

    public virtual ICollection<GoodsReceipts> GoodsReceiptsReceivedByUser { get; set; } = new List<GoodsReceipts>();

    public virtual ICollection<GoodsReceipts> GoodsReceiptsUpdatedByNavigation { get; set; } = new List<GoodsReceipts>();

    public virtual ICollection<InventoryAdjustmentLines> InventoryAdjustmentLines { get; set; } = new List<InventoryAdjustmentLines>();

    public virtual ICollection<InventoryAdjustments> InventoryAdjustmentsApprovedByUser { get; set; } = new List<InventoryAdjustments>();

    public virtual ICollection<InventoryAdjustments> InventoryAdjustmentsCreatedByNavigation { get; set; } = new List<InventoryAdjustments>();

    public virtual ICollection<InventoryAdjustments> InventoryAdjustmentsUpdatedByNavigation { get; set; } = new List<InventoryAdjustments>();

    public virtual ICollection<Invoices> InvoicesCreatedByNavigation { get; set; } = new List<Invoices>();

    public virtual ICollection<Invoices> InvoicesUpdatedByNavigation { get; set; } = new List<Invoices>();

    public virtual ICollection<LoyaltyTransactions> LoyaltyTransactionsCreatedByNavigation { get; set; } = new List<LoyaltyTransactions>();

    public virtual ICollection<LoyaltyTransactions> LoyaltyTransactionsUpdatedByNavigation { get; set; } = new List<LoyaltyTransactions>();

    public virtual ICollection<MediaAssets> MediaAssetsCreatedByNavigation { get; set; } = new List<MediaAssets>();

    public virtual ICollection<MediaAssets> MediaAssetsUpdatedByNavigation { get; set; } = new List<MediaAssets>();

    public virtual ICollection<MediaAssets> MediaAssetsUploadedByNavigation { get; set; } = new List<MediaAssets>();

    public virtual ICollection<Menus> MenusCreatedByNavigation { get; set; } = new List<Menus>();

    public virtual ICollection<Menus> MenusUpdatedByNavigation { get; set; } = new List<Menus>();

    public virtual ICollection<Notifications> NotificationsCreatedByNavigation { get; set; } = new List<Notifications>();

    public virtual ICollection<Notifications> NotificationsUser { get; set; } = new List<Notifications>();

    public virtual ICollection<OrderBundleSelections> OrderBundleSelections { get; set; } = new List<OrderBundleSelections>();

    public virtual ICollection<OrderItemTaxes> OrderItemTaxes { get; set; } = new List<OrderItemTaxes>();

    public virtual ICollection<OrderItems> OrderItemsCreatedByNavigation { get; set; } = new List<OrderItems>();

    public virtual ICollection<OrderItems> OrderItemsUpdatedByNavigation { get; set; } = new List<OrderItems>();

    public virtual ICollection<OrderReturnItems> OrderReturnItemsCreatedByNavigation { get; set; } = new List<OrderReturnItems>();

    public virtual ICollection<OrderReturnItems> OrderReturnItemsUpdatedByNavigation { get; set; } = new List<OrderReturnItems>();

    public virtual ICollection<Orders> OrdersCreatedByNavigation { get; set; } = new List<Orders>();

    public virtual ICollection<Orders> OrdersUpdatedByNavigation { get; set; } = new List<Orders>();

    public virtual ICollection<Orders> OrdersUser { get; set; } = new List<Orders>();

    public virtual ICollection<PaymentGateways> PaymentGatewaysCreatedByNavigation { get; set; } = new List<PaymentGateways>();

    public virtual ICollection<PaymentGateways> PaymentGatewaysUpdatedByNavigation { get; set; } = new List<PaymentGateways>();

    public virtual ICollection<Payments> PaymentsCreatedByNavigation { get; set; } = new List<Payments>();

    public virtual ICollection<Payments> PaymentsUpdatedByNavigation { get; set; } = new List<Payments>();

    public virtual ICollection<Permissions> PermissionsCreatedByNavigation { get; set; } = new List<Permissions>();

    public virtual ICollection<Permissions> PermissionsUpdatedByNavigation { get; set; } = new List<Permissions>();

    public virtual ICollection<PickupPoints> PickupPointsCreatedByNavigation { get; set; } = new List<PickupPoints>();

    public virtual ICollection<PickupPoints> PickupPointsUpdatedByNavigation { get; set; } = new List<PickupPoints>();

    public virtual ICollection<PosCounters> PosCountersCreatedByNavigation { get; set; } = new List<PosCounters>();

    public virtual ICollection<PosCounters> PosCountersUpdatedByNavigation { get; set; } = new List<PosCounters>();

    public virtual ICollection<PosPaymentTenders> PosPaymentTendersCreatedByNavigation { get; set; } = new List<PosPaymentTenders>();

    public virtual ICollection<PosPaymentTenders> PosPaymentTendersUpdatedByNavigation { get; set; } = new List<PosPaymentTenders>();

    public virtual ICollection<PosTerminals> PosTerminalsCreatedByNavigation { get; set; } = new List<PosTerminals>();

    public virtual ICollection<PosTerminals> PosTerminalsUpdatedByNavigation { get; set; } = new List<PosTerminals>();

    public virtual ICollection<PosTransactionBundleSelections> PosTransactionBundleSelections { get; set; } = new List<PosTransactionBundleSelections>();

    public virtual ICollection<PosTransactionLineTaxes> PosTransactionLineTaxes { get; set; } = new List<PosTransactionLineTaxes>();

    public virtual ICollection<PosTransactionLines> PosTransactionLinesCreatedByNavigation { get; set; } = new List<PosTransactionLines>();

    public virtual ICollection<PosTransactionLines> PosTransactionLinesUpdatedByNavigation { get; set; } = new List<PosTransactionLines>();

    public virtual ICollection<PosTransactionReturnLines> PosTransactionReturnLines { get; set; } = new List<PosTransactionReturnLines>();

    public virtual ICollection<PosTransactionReturns> PosTransactionReturnsCreatedByNavigation { get; set; } = new List<PosTransactionReturns>();

    public virtual ICollection<PosTransactionReturns> PosTransactionReturnsCreatedByUser { get; set; } = new List<PosTransactionReturns>();

    public virtual ICollection<PosTransactionReturns> PosTransactionReturnsUpdatedByNavigation { get; set; } = new List<PosTransactionReturns>();

    public virtual ICollection<PosTransactions> PosTransactionsCashier { get; set; } = new List<PosTransactions>();

    public virtual ICollection<PosTransactions> PosTransactionsCreatedByNavigation { get; set; } = new List<PosTransactions>();

    public virtual ICollection<PosTransactions> PosTransactionsUpdatedByNavigation { get; set; } = new List<PosTransactions>();

    public virtual ICollection<PosTransactions> PosTransactionsVoidedByNavigation { get; set; } = new List<PosTransactions>();

    public virtual ICollection<PriceListItems> PriceListItemsCreatedByNavigation { get; set; } = new List<PriceListItems>();

    public virtual ICollection<PriceListItems> PriceListItemsUpdatedByNavigation { get; set; } = new List<PriceListItems>();

    public virtual ICollection<PriceLists> PriceListsCreatedByNavigation { get; set; } = new List<PriceLists>();

    public virtual ICollection<PriceLists> PriceListsUpdatedByNavigation { get; set; } = new List<PriceLists>();

    public virtual ICollection<ProductAttributeLinks> ProductAttributeLinks { get; set; } = new List<ProductAttributeLinks>();

    public virtual ICollection<ProductBatches> ProductBatchesCreatedByNavigation { get; set; } = new List<ProductBatches>();

    public virtual ICollection<ProductBatches> ProductBatchesUpdatedByNavigation { get; set; } = new List<ProductBatches>();

    public virtual ICollection<ProductCollectionItems> ProductCollectionItems { get; set; } = new List<ProductCollectionItems>();

    public virtual ICollection<ProductCollections> ProductCollectionsCreatedByNavigation { get; set; } = new List<ProductCollections>();

    public virtual ICollection<ProductCollections> ProductCollectionsUpdatedByNavigation { get; set; } = new List<ProductCollections>();

    public virtual ICollection<ProductImages> ProductImagesCreatedByNavigation { get; set; } = new List<ProductImages>();

    public virtual ICollection<ProductImages> ProductImagesUpdatedByNavigation { get; set; } = new List<ProductImages>();

    public virtual ICollection<ProductMedia> ProductMediaCreatedByNavigation { get; set; } = new List<ProductMedia>();

    public virtual ICollection<ProductMedia> ProductMediaUpdatedByNavigation { get; set; } = new List<ProductMedia>();

    public virtual ICollection<ProductMedia> ProductMediaUploadedByNavigation { get; set; } = new List<ProductMedia>();

    public virtual ICollection<ProductPriceHistories> ProductPriceHistoriesChangedByUser { get; set; } = new List<ProductPriceHistories>();

    public virtual ICollection<ProductPriceHistories> ProductPriceHistoriesCreatedByNavigation { get; set; } = new List<ProductPriceHistories>();

    public virtual ICollection<ProductSpecificationValues> ProductSpecificationValuesCreatedByNavigation { get; set; } = new List<ProductSpecificationValues>();

    public virtual ICollection<ProductSpecificationValues> ProductSpecificationValuesUpdatedByNavigation { get; set; } = new List<ProductSpecificationValues>();

    public virtual ICollection<ProductSpecifications> ProductSpecifications { get; set; } = new List<ProductSpecifications>();

    public virtual ICollection<ProductSupplierLinks> ProductSupplierLinksCreatedByNavigation { get; set; } = new List<ProductSupplierLinks>();

    public virtual ICollection<ProductSupplierLinks> ProductSupplierLinksUpdatedByNavigation { get; set; } = new List<ProductSupplierLinks>();

    public virtual ICollection<ProductTags> ProductTags { get; set; } = new List<ProductTags>();

    public virtual ICollection<ProductTaxRates> ProductTaxRatesCreatedByNavigation { get; set; } = new List<ProductTaxRates>();

    public virtual ICollection<ProductTaxRates> ProductTaxRatesUpdatedByNavigation { get; set; } = new List<ProductTaxRates>();

    public virtual ICollection<ProductVariants> ProductVariantsCreatedByNavigation { get; set; } = new List<ProductVariants>();

    public virtual ICollection<ProductVariants> ProductVariantsUpdatedByNavigation { get; set; } = new List<ProductVariants>();

    public virtual ICollection<Products> ProductsCreatedByNavigation { get; set; } = new List<Products>();

    public virtual ICollection<Products> ProductsUpdatedByNavigation { get; set; } = new List<Products>();

    public virtual ICollection<PurchaseOrderLineTaxes> PurchaseOrderLineTaxes { get; set; } = new List<PurchaseOrderLineTaxes>();

    public virtual ICollection<PurchaseOrderLines> PurchaseOrderLinesCreatedByNavigation { get; set; } = new List<PurchaseOrderLines>();

    public virtual ICollection<PurchaseOrderLines> PurchaseOrderLinesUpdatedByNavigation { get; set; } = new List<PurchaseOrderLines>();

    public virtual ICollection<PurchaseOrders> PurchaseOrdersApprovedByUser { get; set; } = new List<PurchaseOrders>();

    public virtual ICollection<PurchaseOrders> PurchaseOrdersCreatedByNavigation { get; set; } = new List<PurchaseOrders>();

    public virtual ICollection<PurchaseOrders> PurchaseOrdersCreatedByUser { get; set; } = new List<PurchaseOrders>();

    public virtual ICollection<PurchaseOrders> PurchaseOrdersUpdatedByNavigation { get; set; } = new List<PurchaseOrders>();

    public virtual ICollection<PurchaseReturnLines> PurchaseReturnLines { get; set; } = new List<PurchaseReturnLines>();

    public virtual ICollection<PurchaseReturns> PurchaseReturnsCreatedByNavigation { get; set; } = new List<PurchaseReturns>();

    public virtual ICollection<PurchaseReturns> PurchaseReturnsCreatedByUser { get; set; } = new List<PurchaseReturns>();

    public virtual ICollection<PurchaseReturns> PurchaseReturnsUpdatedByNavigation { get; set; } = new List<PurchaseReturns>();

    public virtual ICollection<QuoteItems> QuoteItemsCreatedByNavigation { get; set; } = new List<QuoteItems>();

    public virtual ICollection<QuoteItems> QuoteItemsUpdatedByNavigation { get; set; } = new List<QuoteItems>();

    public virtual ICollection<Quotes> QuotesCreatedByNavigation { get; set; } = new List<Quotes>();

    public virtual ICollection<Quotes> QuotesUpdatedByNavigation { get; set; } = new List<Quotes>();

    public virtual ICollection<RefundRequests> RefundRequestsCreatedByNavigation { get; set; } = new List<RefundRequests>();

    public virtual ICollection<RefundRequests> RefundRequestsUpdatedByNavigation { get; set; } = new List<RefundRequests>();

    public virtual ICollection<ReorderRules> ReorderRulesCreatedByNavigation { get; set; } = new List<ReorderRules>();

    public virtual ICollection<ReorderRules> ReorderRulesNotifyUser { get; set; } = new List<ReorderRules>();

    public virtual ICollection<ReorderRules> ReorderRulesUpdatedByNavigation { get; set; } = new List<ReorderRules>();

    public virtual ICollection<Returns> ReturnsCreatedByNavigation { get; set; } = new List<Returns>();

    public virtual ICollection<Returns> ReturnsProcessedByUser { get; set; } = new List<Returns>();

    public virtual ICollection<Returns> ReturnsUpdatedByNavigation { get; set; } = new List<Returns>();

    public virtual ICollection<ReviewHelpfulness> ReviewHelpfulness { get; set; } = new List<ReviewHelpfulness>();

    public virtual ICollection<Reviews> ReviewsCreatedByNavigation { get; set; } = new List<Reviews>();

    public virtual ICollection<Reviews> ReviewsUpdatedByNavigation { get; set; } = new List<Reviews>();

    public virtual ICollection<RoleMenus> RoleMenusCreatedByNavigation { get; set; } = new List<RoleMenus>();

    public virtual ICollection<RoleMenus> RoleMenusUpdatedByNavigation { get; set; } = new List<RoleMenus>();

    public virtual ICollection<Sellers> SellersApprovedByUser { get; set; } = new List<Sellers>();

    public virtual ICollection<Sellers> SellersCreatedByNavigation { get; set; } = new List<Sellers>();

    public virtual ICollection<Sellers> SellersUpdatedByNavigation { get; set; } = new List<Sellers>();

    public virtual Sellers? SellersUser { get; set; }

    public virtual ICollection<Shipments> ShipmentsCreatedByNavigation { get; set; } = new List<Shipments>();

    public virtual ICollection<Shipments> ShipmentsUpdatedByNavigation { get; set; } = new List<Shipments>();

    public virtual ICollection<ShippingCarriers> ShippingCarriersCreatedByNavigation { get; set; } = new List<ShippingCarriers>();

    public virtual ICollection<ShippingCarriers> ShippingCarriersUpdatedByNavigation { get; set; } = new List<ShippingCarriers>();

    public virtual ICollection<ShippingMethods> ShippingMethodsCreatedByNavigation { get; set; } = new List<ShippingMethods>();

    public virtual ICollection<ShippingMethods> ShippingMethodsUpdatedByNavigation { get; set; } = new List<ShippingMethods>();

    public virtual ICollection<StaticPages> StaticPagesCreatedByNavigation { get; set; } = new List<StaticPages>();

    public virtual ICollection<StaticPages> StaticPagesUpdatedByNavigation { get; set; } = new List<StaticPages>();

    public virtual ICollection<StockItems> StockItemsCountedByUser { get; set; } = new List<StockItems>();

    public virtual ICollection<StockItems> StockItemsCreatedByNavigation { get; set; } = new List<StockItems>();

    public virtual ICollection<StockItems> StockItemsUpdatedByNavigation { get; set; } = new List<StockItems>();

    public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();

    public virtual ICollection<StockTransferLines> StockTransferLines { get; set; } = new List<StockTransferLines>();

    public virtual ICollection<StockTransfers> StockTransfersCreatedByNavigation { get; set; } = new List<StockTransfers>();

    public virtual ICollection<StockTransfers> StockTransfersCreatedByUser { get; set; } = new List<StockTransfers>();

    public virtual ICollection<StockTransfers> StockTransfersUpdatedByNavigation { get; set; } = new List<StockTransfers>();

    public virtual ICollection<Suppliers> SuppliersCreatedByNavigation { get; set; } = new List<Suppliers>();

    public virtual ICollection<Suppliers> SuppliersUpdatedByNavigation { get; set; } = new List<Suppliers>();

    public virtual ICollection<SupportTicketMessages> SupportTicketMessagesCreatedByNavigation { get; set; } = new List<SupportTicketMessages>();

    public virtual ICollection<SupportTicketMessages> SupportTicketMessagesSender { get; set; } = new List<SupportTicketMessages>();

    public virtual ICollection<SupportTickets> SupportTicketsAssignedTo { get; set; } = new List<SupportTickets>();

    public virtual ICollection<SupportTickets> SupportTicketsCreatedByNavigation { get; set; } = new List<SupportTickets>();

    public virtual ICollection<SupportTickets> SupportTicketsUpdatedByNavigation { get; set; } = new List<SupportTickets>();

    public virtual ICollection<SupportTickets> SupportTicketsUser { get; set; } = new List<SupportTickets>();

    public virtual ICollection<Tags> TagsCreatedByNavigation { get; set; } = new List<Tags>();

    public virtual ICollection<Tags> TagsUpdatedByNavigation { get; set; } = new List<Tags>();

    public virtual ICollection<TaxRates> TaxRatesCreatedByNavigation { get; set; } = new List<TaxRates>();

    public virtual ICollection<TaxRates> TaxRatesUpdatedByNavigation { get; set; } = new List<TaxRates>();

    public virtual ICollection<Units> UnitsCreatedByNavigation { get; set; } = new List<Units>();

    public virtual ICollection<Units> UnitsUpdatedByNavigation { get; set; } = new List<Units>();

    public virtual ICollection<UserClaims> UserClaims { get; set; } = new List<UserClaims>();

    public virtual ICollection<UserLogins> UserLogins { get; set; } = new List<UserLogins>();

    public virtual ICollection<UserRefreshTokens> UserRefreshTokensCreatedByNavigation { get; set; } = new List<UserRefreshTokens>();

    public virtual ICollection<UserRefreshTokens> UserRefreshTokensUser { get; set; } = new List<UserRefreshTokens>();

    public virtual ICollection<UserTokens> UserTokens { get; set; } = new List<UserTokens>();

    public virtual ICollection<VariantAttributeMatrix> VariantAttributeMatrix { get; set; } = new List<VariantAttributeMatrix>();

    public virtual ICollection<VariantAttributeOptions> VariantAttributeOptions { get; set; } = new List<VariantAttributeOptions>();

    public virtual ICollection<WalletTransactions> WalletTransactionsCreatedByNavigation { get; set; } = new List<WalletTransactions>();

    public virtual ICollection<WalletTransactions> WalletTransactionsUpdatedByNavigation { get; set; } = new List<WalletTransactions>();

    public virtual ICollection<Warehouses> WarehousesCreatedByNavigation { get; set; } = new List<Warehouses>();

    public virtual ICollection<Warehouses> WarehousesUpdatedByNavigation { get; set; } = new List<Warehouses>();

    public virtual ICollection<WishlistItems> WishlistItemsCreatedByNavigation { get; set; } = new List<WishlistItems>();

    public virtual ICollection<WishlistItems> WishlistItemsUpdatedByNavigation { get; set; } = new List<WishlistItems>();

    public virtual ICollection<Wishlists> WishlistsCreatedByNavigation { get; set; } = new List<Wishlists>();

    public virtual ICollection<Wishlists> WishlistsUpdatedByNavigation { get; set; } = new List<Wishlists>();

    public virtual ICollection<Wishlists> WishlistsUser { get; set; } = new List<Wishlists>();

    public virtual ICollection<Roles> Role { get; set; } = new List<Roles>();
}
