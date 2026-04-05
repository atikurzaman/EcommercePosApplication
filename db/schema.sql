-- ═══════════════════════════════════════════════════════════════════════════
--  UNIFIED NORMALIZED DATABASE SCHEMA  v4.0
--  Platform : SQL Server 2019+ / Azure SQL Database
--  Collation : SQL_Latin1_General_CP1_CI_AS
--
--  NORMALIZATION STANDARD
--  ──────────────────────
--  1st Normal Form (1NF)
--    • Every column holds a single atomic value — no CSV lists, no arrays.
--    • Each row is uniquely identified by its primary key.
--    • No repeating groups: derived/computed values (ratings, totals) live
--      in views only, never as stored columns.
--
--  2nd Normal Form (2NF)
--    • Every non-key column depends on the WHOLE primary key, not just part.
--    • Pure junction tables (e.g. ProductTags) carry no non-key payload — only
--      the two FK columns that form the composite PK.
--
--  3rd Normal Form (3NF)
--    • No transitive dependencies: a non-key column must depend on the PK,
--      not on another non-key column.
--    • Status / type values are FK references to lookup tables, not raw strings.
--    • Computed data that derives from other tables is never stored redundantly.
--
--  PRIMARY KEY CONVENTION
--  ──────────────────────
--    • All domain tables use UNIQUEIDENTIFIER (GUID) DEFAULT NEWSEQUENTIALID()
--      as the primary key.  Sequential GUIDs avoid page-split fragmentation
--      while retaining globally-unique, merge-friendly identifiers.
--    • Composite PKs are used only for pure junction tables (Section 6 +).
--    • Lookup / enum tables use a short NVARCHAR code as the natural key.
--
--  ROWVERSION
--  ──────────
--    • All editable domain tables carry a RowVersion column for optimistic
--      concurrency.  Lookup tables, junction tables, blob tables, identity
--      tables and the immutable AuditLogs table are exempt.
--
--  MONEY / PRECISION TYPES
--  ───────────────────────
--    • DECIMAL(18,2)  — all monetary amounts
--    • DECIMAL(9,4)   — tax / discount percentages
--    • DECIMAL(10,7)  — GPS coordinates
--    • DECIMAL(8,3)   — weights in kg
--
--  SOFT DELETE & AUDIT COLUMNS
--  ────────────────────────────
--    • IsDeleted BIT DEFAULT 0 on all domain tables.
--    • CreatedAt, CreatedBy, UpdatedAt, UpdatedBy on all domain tables.
--    • CreatedBy / UpdatedBy reference Users(Id).
--
--  TABLE COUNT : 115 tables  |  4 views  |  43+ indexes
-- ═══════════════════════════════════════════════════════════════════════════

USE master;
GO
BEGIN TRY
    IF DB_ID('EcommercePosDb') IS NULL
        CREATE DATABASE EcommercePosDb COLLATE SQL_Latin1_General_CP1_CI_AS;
END TRY
BEGIN CATCH END CATCH
GO
USE EcommercePosDb;
GO
SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON; SET NOCOUNT ON;
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 1  LOOKUP / ENUM TABLES
-- ───────────────────────────────────────────────────────────────────────────
-- 3NF: Status and type values are stored as reference tables with a short
-- code as natural PK, not as raw strings with CHECK constraints.  This gives
-- FK integrity, extensibility without DDL, and admin-editable labels.
-- These tables are seeded once and cached by the application layer.
-- ═══════════════════════════════════════════════════════════════════════════

-- OrderStatuses: Defines every valid state an order can occupy.
-- 1NF: Each row is one discrete state with a single code as its natural key.
CREATE TABLE OrderStatuses (
    StatusCode  NVARCHAR(30)  NOT NULL,   -- machine-readable key used in FK references
    DisplayName NVARCHAR(80)  NOT NULL,   -- UI label shown to customers and staff
    Description NVARCHAR(300) NULL,       -- longer explanation for admin tooltips
    SortOrder   TINYINT       NOT NULL DEFAULT 0,   -- controls display sequence
    IsTerminal  BIT           NOT NULL DEFAULT 0,   -- 1 = no further transitions allowed
    CONSTRAINT PK_OrderStatuses PRIMARY KEY (StatusCode)
);

-- PaymentStatuses: All possible states for a payment record.
CREATE TABLE PaymentStatuses (
    StatusCode  NVARCHAR(30) NOT NULL,   -- e.g. Pending, Completed, Failed, Refunded
    DisplayName NVARCHAR(80) NOT NULL,
    CONSTRAINT PK_PaymentStatuses PRIMARY KEY (StatusCode)
);

-- PaymentMethods: Every accepted payment channel across POS and e-commerce.
-- 3NF: Replaces raw PaymentMethod strings scattered across multiple tables.
CREATE TABLE PaymentMethods (
    MethodCode  NVARCHAR(40) NOT NULL,   -- e.g. Cash, bKash, Card, COD, Wallet
    DisplayName NVARCHAR(80) NOT NULL,
    IsOnline    BIT          NOT NULL DEFAULT 1,   -- 0 = cash / COD (physical)
    IsActive    BIT          NOT NULL DEFAULT 1,
    SortOrder   TINYINT      NOT NULL DEFAULT 0,
    CONSTRAINT PK_PaymentMethods PRIMARY KEY (MethodCode)
);

-- ShipmentStatuses: States for an outbound shipment record.
CREATE TABLE ShipmentStatuses (
    StatusCode  NVARCHAR(30) NOT NULL,   -- e.g. Pending, Packed, Dispatched, Delivered
    DisplayName NVARCHAR(80) NOT NULL,
    SortOrder   TINYINT      NOT NULL DEFAULT 0,
    CONSTRAINT PK_ShipmentStatuses PRIMARY KEY (StatusCode)
);

-- ReturnStatuses: States shared by Returns and RefundRequests tables.
CREATE TABLE ReturnStatuses (
    StatusCode  NVARCHAR(30) NOT NULL,   -- e.g. Requested, Approved, Received, Refunded
    DisplayName NVARCHAR(80) NOT NULL,
    SortOrder   TINYINT      NOT NULL DEFAULT 0,
    CONSTRAINT PK_ReturnStatuses PRIMARY KEY (StatusCode)
);

-- StockMovementTypes: Classifies every inventory movement direction and cause.
-- 3NF: Replaces bare string MovementType columns across stock tables.
CREATE TABLE StockMovementTypes (
    TypeCode    NVARCHAR(30) NOT NULL,   -- e.g. Purchase, Sale, Return, Adjustment
    DisplayName NVARCHAR(80) NOT NULL,
    IsInbound   BIT          NOT NULL DEFAULT 1,   -- 1 = stock increases; 0 = stock decreases
    CONSTRAINT PK_StockMovementTypes PRIMARY KEY (TypeCode)
);

-- DiscountTypes: Defines the mechanic of a promotional discount.
CREATE TABLE DiscountTypes (
    TypeCode    NVARCHAR(30) NOT NULL,   -- e.g. Percentage, Fixed, BOGO, FreeShipping
    DisplayName NVARCHAR(80) NOT NULL,
    CONSTRAINT PK_DiscountTypes PRIMARY KEY (TypeCode)
);

-- CustomerTiers: Loyalty programme tier levels (Bronze → Platinum).
-- 3NF: Tier-specific discount rates and multipliers live here, not on Customers.
CREATE TABLE CustomerTiers (
    TierCode         NVARCHAR(20)  NOT NULL,   -- e.g. Bronze, Silver, Gold, Platinum
    DisplayName      NVARCHAR(80)  NOT NULL,
    MinLifetimeSpend DECIMAL(18,2) NOT NULL DEFAULT 0,    -- spend threshold to qualify
    DiscountPct      DECIMAL(5,2)  NOT NULL DEFAULT 0,    -- automatic tier discount applied at checkout
    PointsMultiplier DECIMAL(5,2)  NOT NULL DEFAULT 1.0,  -- loyalty point earn multiplier
    SortOrder        TINYINT       NOT NULL DEFAULT 0,
    CONSTRAINT PK_CustomerTiers PRIMARY KEY (TierCode)
);

-- ProductConditions: Physical condition of a product listing.
CREATE TABLE ProductConditions (
    ConditionCode NVARCHAR(20) NOT NULL,   -- e.g. New, Refurbished, Used, Damaged
    DisplayName   NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_ProductConditions PRIMARY KEY (ConditionCode)
);

-- WishlistTypes: Classifies the purpose of a customer wishlist.
CREATE TABLE WishlistTypes (
    TypeCode    NVARCHAR(20) NOT NULL,   -- e.g. Personal, Registry, Public
    DisplayName NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_WishlistTypes PRIMARY KEY (TypeCode)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 2  IDENTITY — USERS, ROLES & AUTH INFRASTRUCTURE
-- ───────────────────────────────────────────────────────────────────────────
-- All tables use UNIQUEIDENTIFIER PKs.  The ASP.NET Identity framework is
-- ConcurrencyStamp is kept on Users and Roles as the framework writes it.
-- ═══════════════════════════════════════════════════════════════════════════

-- Users: Core authentication identity extended with domain-level columns.
-- 2NF: Every extension column depends solely on Id (scalar PK); no partial
--      dependency is possible.
CREATE TABLE Users (
    Id                      UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),  -- GUID PK; EF Identity configured for GUID keys
    UserName                NVARCHAR(256)    NULL,
    NormalizedUserName      NVARCHAR(256)    NULL,
    Email                   NVARCHAR(256)    NULL,
    NormalizedEmail         NVARCHAR(256)    NULL,
    EmailConfirmed          BIT              NOT NULL DEFAULT 0,      -- 1 once the email verification link is clicked
    PasswordHash            NVARCHAR(MAX)    NULL,                    -- PBKDF2 / bcrypt hash; never store plaintext
    SecurityStamp           NVARCHAR(MAX)    NULL,                    -- rotated on password change; invalidates old tokens
    ConcurrencyStamp        NVARCHAR(MAX)    NULL,                    -- framework optimistic-concurrency stamp
    PhoneNumber             NVARCHAR(30)     NULL,
    PhoneNumberConfirmed    BIT              NOT NULL DEFAULT 0,
    TwoFactorEnabled        BIT              NOT NULL DEFAULT 0,
    LockoutEnd              DATETIMEOFFSET   NULL,                    -- NULL means the account is not currently locked
    LockoutEnabled          BIT              NOT NULL DEFAULT 1,
    AccessFailedCount       INT              NOT NULL DEFAULT 0,      -- resets to 0 after a successful login
    -- Domain extension columns
    FirstName               NVARCHAR(100)    NULL,
    LastName                NVARCHAR(100)    NULL,
    AvatarUrl               NVARCHAR(500)    NULL,                    -- CDN URL; no blob FK avoids a circular dependency
    IsActive                BIT              NOT NULL DEFAULT 1,      -- 0 = account suspended by admin
    PreferredLanguage       NCHAR(5)         NOT NULL DEFAULT 'en',   -- BCP-47 tag e.g. en, bn, ar
    TimeZone                NVARCHAR(60)     NOT NULL DEFAULT 'Asia/Dhaka',  -- IANA tz database name
    LastLoginAt             DATETIME2        NULL,                    -- stamped on every successful authentication
    LastPasswordChangedAt   DATETIME2        NULL,                    -- used to enforce periodic password rotation policy
    EmailVerificationToken  NVARCHAR(MAX)    NULL,                    -- one-time token sent in the verification email
    EmailVerificationExpiry DATETIME2        NULL,                    -- token is invalid after this timestamp
    PasswordResetToken      NVARCHAR(MAX)    NULL,                    -- one-time token for password reset flow
    PasswordResetExpiry     DATETIME2        NULL,
    CreatedAt               DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Users PRIMARY KEY (Id)
);
CREATE UNIQUE INDEX UX_Users_UserName ON Users(NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
CREATE UNIQUE INDEX UX_Users_Email    ON Users(NormalizedEmail)    WHERE NormalizedEmail    IS NOT NULL;

-- Roles: Application security roles (Admin, Cashier, StoreManager, Seller…).
CREATE TABLE Roles (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),  -- GUID PK
    Name             NVARCHAR(256)    NULL,
    NormalizedName   NVARCHAR(256)    NULL,
    ConcurrencyStamp NVARCHAR(MAX)    NULL,
    Description      NVARCHAR(255)    NULL,    -- human-readable description shown in admin role management
    IsActive         BIT              NOT NULL DEFAULT 1,
    CONSTRAINT PK_Roles PRIMARY KEY (Id)
);
CREATE UNIQUE INDEX UX_Roles_Name ON Roles(NormalizedName) WHERE NormalizedName IS NOT NULL;

-- UserRoles: M:N role assignments.
-- 2NF: No non-key columns; the composite (UserId, RoleId) is the entire table.
CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    RoleId UNIQUEIDENTIFIER NOT NULL REFERENCES Roles(Id) ON DELETE CASCADE,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId)
);

-- UserClaims: Per-user fine-grained claims (key-value pairs).
CREATE TABLE UserClaims (
    Id         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    UserId     UNIQUEIDENTIFIER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    ClaimType  NVARCHAR(MAX)    NULL,    -- claim URI e.g. "http://schemas.xmlsoap.org/claims/Group"
    ClaimValue NVARCHAR(MAX)    NULL,
    CONSTRAINT PK_UserClaims PRIMARY KEY (Id)
);

-- RoleClaims: Claims automatically inherited by all members of a role.
CREATE TABLE RoleClaims (
    Id         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    RoleId     UNIQUEIDENTIFIER NOT NULL REFERENCES Roles(Id) ON DELETE CASCADE,
    ClaimType  NVARCHAR(MAX)    NULL,
    ClaimValue NVARCHAR(MAX)    NULL,
    CONSTRAINT PK_RoleClaims PRIMARY KEY (Id)
);

-- UserLogins: External OAuth / OIDC provider links (Google, Facebook…).
-- Composite PK on provider + key is the natural identity for this table.
CREATE TABLE UserLogins (
    Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    LoginProvider       NVARCHAR(128)    NOT NULL,   -- e.g. "Google", "Facebook"
    ProviderKey         NVARCHAR(128)    NOT NULL,   -- subject identifier issued by the external provider
    ProviderDisplayName NVARCHAR(MAX)    NULL,        -- friendly label shown in linked-accounts UI
    UserId              UNIQUEIDENTIFIER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT PK_UserLogins PRIMARY KEY (Id),
    CONSTRAINT UX_UserLogins_Provider UNIQUE (LoginProvider, ProviderKey)
);

-- UserTokens: 2FA tokens, authenticator keys, recovery codes.
CREATE TABLE UserTokens (
    Id            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    UserId        UNIQUEIDENTIFIER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    LoginProvider NVARCHAR(128)    NOT NULL,   -- issuing provider or "Local"
    Name          NVARCHAR(128)    NOT NULL,   -- token purpose e.g. "AuthenticatorKey", "RecoveryCode"
    Value         NVARCHAR(MAX)    NULL,        -- token value; sensitive — protect at application layer
    CONSTRAINT PK_UserTokens PRIMARY KEY (Id),
    CONSTRAINT UX_UserTokens UNIQUE (UserId, LoginProvider, Name)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 3  CUSTOM RBAC & SECURITY
-- ═══════════════════════════════════════════════════════════════════════════

-- Permissions: Named atomic capabilities used by the RBAC layer.
-- 3NF: PermissionCode is the natural candidate key; Module is a grouping
--      attribute that depends on the code alone, not on any other column.
CREATE TABLE Permissions (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PermissionCode NVARCHAR(100)    NOT NULL,   -- machine key e.g. "products.create"
    Name           NVARCHAR(150)    NOT NULL,   -- display label e.g. "Create Products"
    Module         NVARCHAR(100)    NOT NULL,   -- grouping e.g. "Products", "Orders", "POS"
    Description    NVARCHAR(500)    NULL,
    IsActive       BIT              NOT NULL DEFAULT 1,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt      DATETIME2        NULL,
    UpdatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_Permissions PRIMARY KEY (Id),
    CONSTRAINT UX_Permissions_Code UNIQUE (PermissionCode)
);

-- RolePermissions: Grants a named permission to an identity role.
-- 2NF: IsGranted is the only non-key attribute and it depends on the full
--      composite key (RoleId + PermissionId), not just one part.
CREATE TABLE RolePermissions (
    RoleId       UNIQUEIDENTIFIER    NOT NULL REFERENCES Roles      (Id) ON DELETE CASCADE,
    PermissionId UNIQUEIDENTIFIER NOT NULL REFERENCES Permissions(Id) ON DELETE CASCADE,
    IsGranted    BIT              NOT NULL DEFAULT 1,   -- explicit deny supported via 0
    CONSTRAINT PK_RolePermissions PRIMARY KEY (RoleId, PermissionId)
);

-- Menus: Hierarchical navigation tree for admin panel access control.
-- ParentMenuId enables unlimited nesting; MenuLevel is a denorm shortcut
-- for rendering depth without recursive CTEs.
CREATE TABLE Menus (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ParentMenuId   UNIQUEIDENTIFIER NULL REFERENCES Menus(Id),
    MenuCode       NVARCHAR(50)     NOT NULL,   -- slug-style machine key e.g. "products-list"
    MenuName       NVARCHAR(100)    NOT NULL,   -- internal name for configuration
    DisplayName    NVARCHAR(150)    NOT NULL,   -- label rendered in the sidebar
    MenuUrl        NVARCHAR(300)    NULL,        -- relative URL for SPA routing
    IconClass      NVARCHAR(100)    NULL,        -- CSS class e.g. "fa fa-box"
    DisplayOrder   INT              NOT NULL DEFAULT 0,
    MenuLevel      TINYINT          NOT NULL DEFAULT 1,   -- 1=root, 2=sub, 3=leaf
    PermissionCode NVARCHAR(100)    NULL,        -- if set, user must hold this permission to see item
    IsActive       BIT              NOT NULL DEFAULT 1,
    IsVisible      BIT              NOT NULL DEFAULT 1,   -- 0 hides from nav but keeps route alive
    IsExternalLink BIT              NOT NULL DEFAULT 0,
    OpenInNewTab   BIT              NOT NULL DEFAULT 0,
    Description    NVARCHAR(255)    NULL,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt      DATETIME2        NULL,
    UpdatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_Menus PRIMARY KEY (Id),
    CONSTRAINT UX_Menus_Code UNIQUE (MenuCode)
);

-- RoleMenus: Per-role menu visibility and CRUD rights.
-- 2NF: All five permission bits depend on the full (RoleId, MenuId) key.
CREATE TABLE RoleMenus (
    Id         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    RoleId     UNIQUEIDENTIFIER    NOT NULL REFERENCES Roles(Id) ON DELETE CASCADE,
    MenuId     UNIQUEIDENTIFIER NOT NULL REFERENCES Menus(Id) ON DELETE CASCADE,
    CanView    BIT              NOT NULL DEFAULT 1,
    CanAdd     BIT              NOT NULL DEFAULT 0,
    CanEdit    BIT              NOT NULL DEFAULT 0,
    CanDelete  BIT              NOT NULL DEFAULT 0,
    CanApprove BIT              NOT NULL DEFAULT 0,
    CreatedAt  DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt  DATETIME2        NULL,
    UpdatedBy  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CONSTRAINT PK_RoleMenus PRIMARY KEY (Id),
    CONSTRAINT UX_RoleMenus UNIQUE (RoleId, MenuId)
);

-- UserRefreshTokens: JWT refresh token store with revocation and rotation audit.
CREATE TABLE UserRefreshTokens (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    UserId          UNIQUEIDENTIFIER    NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    Token           NVARCHAR(500)    NOT NULL,   -- opaque random token string
    ExpiresAt       DATETIME2        NOT NULL,
    CreatedByIp     NVARCHAR(50)     NULL,        -- client IP at token creation
    RevokedAt       DATETIME2        NULL,        -- NULL = still valid
    RevokedByIp     NVARCHAR(50)     NULL,
    ReplacedByToken NVARCHAR(500)    NULL,        -- token that superseded this one (rotation chain)
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_UserRefreshTokens PRIMARY KEY (Id)
);
CREATE INDEX IX_RefreshTokens_User ON UserRefreshTokens(UserId) WHERE IsDeleted=0;
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 4  CURRENCIES & TAX RATES
-- ═══════════════════════════════════════════════════════════════════════════

-- Currencies: ISO 4217 currency master for multi-currency support.
CREATE TABLE Currencies (
    CurrencyCode   NCHAR(3)      NOT NULL,   -- ISO 4217 code e.g. BDT, USD, GBP
    Name           NVARCHAR(50)  NOT NULL,
    Symbol         NVARCHAR(5)   NOT NULL,   -- rendered symbol e.g. ৳, $, £
    ExchangeRate   DECIMAL(18,6) NOT NULL DEFAULT 1.0,  -- rate relative to base currency
    DecimalPlaces  TINYINT       NOT NULL DEFAULT 2,    -- 0 for BDT (no paisa shown)
    IsBaseCurrency BIT           NOT NULL DEFAULT 0,    -- exactly one row should be 1
    IsActive       BIT           NOT NULL DEFAULT 1,
    CreatedAt      DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy      UNIQUEIDENTIFIER NULL REFERENCES Users(Id),
    UpdatedAt      DATETIME2     NULL,
    UpdatedBy      UNIQUEIDENTIFIER NULL REFERENCES Users(Id),
    CONSTRAINT PK_Currencies PRIMARY KEY (CurrencyCode)
);

-- TaxRates: VAT / GST rate definitions applied to products and order lines.
-- 3NF: IsInclusive, ApplyToShipping, Priority all depend on the PK (Id) only.
CREATE TABLE TaxRates (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TaxCode         NVARCHAR(50)     NOT NULL,   -- short code e.g. BD-VAT-15
    TaxName         NVARCHAR(100)    NOT NULL,   -- display label e.g. "VAT 15%"
    TaxType         NVARCHAR(30)     NOT NULL DEFAULT 'Percentage'
                    CONSTRAINT CK_TaxRates_Type CHECK(TaxType IN('Percentage','Fixed')),
    Rate            DECIMAL(9,4)     NOT NULL CHECK(Rate >= 0),   -- percentage or fixed amount
    IsPercentage    BIT              NOT NULL DEFAULT 1,   -- 1 = Rate is a %; 0 = Rate is a fixed amount
    IsInclusive     BIT              NOT NULL DEFAULT 0,   -- 1 = tax already embedded in the sale price
    IsDefault       BIT              NOT NULL DEFAULT 0,   -- applied when no tax rate is explicitly chosen
    IsActive        BIT              NOT NULL DEFAULT 1,
    Country         NCHAR(2)         NOT NULL DEFAULT 'BD',
    ApplyToShipping BIT              NOT NULL DEFAULT 0,   -- 1 = tax also charged on shipping amount
    Priority        INT              NOT NULL DEFAULT 0,   -- stacking order when multiple rates apply
    Description     NVARCHAR(MAX)    NULL,
    EffectiveFrom   DATETIME2        NULL,   -- rate is not valid before this date
    EffectiveTo     DATETIME2        NULL,   -- rate expires after this date; NULL = no expiry
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_TaxRates PRIMARY KEY (Id),
    CONSTRAINT UX_TaxRates_Code UNIQUE (TaxCode)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 5  WAREHOUSES
-- ═══════════════════════════════════════════════════════════════════════════

-- Warehouses: Physical and virtual inventory / fulfilment sites.
-- SiteType discriminator unifies stores, warehouses, and virtual locations
-- so that a single FK (WarehouseId) can reference any site type without
-- requiring multiple nullable FKs across many tables.
CREATE TABLE Warehouses (
    Id            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Code          NVARCHAR(50)     NOT NULL,   -- short human-readable code e.g. WH-MAIN
    Name          NVARCHAR(150)    NOT NULL,
    SiteType      NVARCHAR(20)     NOT NULL DEFAULT 'Warehouse'
                  CONSTRAINT CK_Warehouses_SiteType CHECK(SiteType IN('Warehouse','Store','Virtual','PickupPoint')),
    ParentId      UNIQUEIDENTIFIER NULL REFERENCES Warehouses(Id),  -- for warehouse zones or sub-areas
    ContactPerson NVARCHAR(150)    NULL,
    ManagerName   NVARCHAR(150)    NULL,
    AddressLine1  NVARCHAR(200)    NULL,
    AddressLine2  NVARCHAR(200)    NULL,
    City          NVARCHAR(100)    NULL,
    Area          NVARCHAR(100)    NULL,        -- sub-district / thana
    State         NVARCHAR(100)    NULL,
    PostalCode    NVARCHAR(20)     NULL,
    Country       NCHAR(2)         NOT NULL DEFAULT 'BD',
    Phone         NVARCHAR(30)     NULL,
    Email         NVARCHAR(150)    NULL,
    Latitude      DECIMAL(10,7)    NULL,        -- GPS latitude for store locator map
    Longitude     DECIMAL(10,7)    NULL,        -- GPS longitude for store locator map
    OpeningTime   TIME(0)          NULL,        -- local trading-hours start
    ClosingTime   TIME(0)          NULL,        -- local trading-hours end
    TaxNumber     NVARCHAR(50)     NULL,        -- VAT/BIN registration number of this location
    IsDefault     BIT              NOT NULL DEFAULT 0,   -- 1 = used when no location specified
    IsActive      BIT              NOT NULL DEFAULT 1,
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt     DATETIME2        NULL,
    UpdatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted     BIT              NOT NULL DEFAULT 0,
    RowVersion    ROWVERSION       NULL,
    CONSTRAINT PK_Warehouses PRIMARY KEY (Id),
    CONSTRAINT UX_Warehouses_Code UNIQUE (Code)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 6  CATALOG REFERENCE DATA
-- ═══════════════════════════════════════════════════════════════════════════

-- Brands: Product manufacturer or brand master.
CREATE TABLE Brands (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name            NVARCHAR(100)    NOT NULL,
    Slug            NVARCHAR(100)    NOT NULL,   -- URL-safe identifier e.g. "samsung"
    Description     NVARCHAR(500)    NULL,
    LogoUrl         NVARCHAR(500)    NULL,        -- CDN URL for brand logo image
    Website         NVARCHAR(200)    NULL,
    CountryOfOrigin NCHAR(2)         NULL,        -- ISO 3166-1 alpha-2 e.g. KR, JP
    IsFeatured      BIT              NOT NULL DEFAULT 0,   -- 1 = shown on featured-brands carousel
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_Brands PRIMARY KEY (Id),
    CONSTRAINT UX_Brands_Slug UNIQUE (Slug)
);

-- Categories: Hierarchical product taxonomy tree.
-- 1NF: ParentCategoryId creates a self-referencing adjacency list.
--      Each row represents exactly one category node.
CREATE TABLE Categories (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ParentCategoryId UNIQUEIDENTIFIER NULL REFERENCES Categories(Id),  -- NULL = root category
    Name             NVARCHAR(100)    NOT NULL,
    Slug             NVARCHAR(100)    NOT NULL,   -- URL segment e.g. "men-shirts"
    Description      NVARCHAR(500)    NULL,
    IconUrl          NVARCHAR(500)    NULL,        -- small icon used in nav menus
    ImageUrl         NVARCHAR(500)    NULL,        -- banner image for category page
    DisplayOrder     INT              NOT NULL DEFAULT 0,
    IsFeatured       BIT              NOT NULL DEFAULT 0,
    IsActive         BIT              NOT NULL DEFAULT 1,
    MetaTitle        NVARCHAR(200)    NULL,        -- <title> tag override for SEO
    MetaDescription  NVARCHAR(500)    NULL,        -- <meta name="description"> for SEO
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Categories PRIMARY KEY (Id),
    CONSTRAINT UX_Categories_Slug UNIQUE (Slug)
);
CREATE INDEX IX_Categories_Parent ON Categories(ParentCategoryId) WHERE ParentCategoryId IS NOT NULL;

-- Suppliers: Vendor master for procurement.
CREATE TABLE Suppliers (
    Id                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    SupplierCode      NVARCHAR(50)     NOT NULL,   -- internal vendor code e.g. SUP-0042
    Name              NVARCHAR(200)    NOT NULL,
    CompanyName       NVARCHAR(150)    NULL,
    ContactPerson     NVARCHAR(150)    NULL,
    Phone             NVARCHAR(30)     NULL,
    AlternatePhone    NVARCHAR(30)     NULL,
    Email             NVARCHAR(256)    NULL,
    AddressLine1      NVARCHAR(200)    NULL,
    AddressLine2      NVARCHAR(200)    NULL,
    City              NVARCHAR(100)    NULL,
    State             NVARCHAR(100)    NULL,
    PostalCode        NVARCHAR(20)     NULL,
    Country           NCHAR(2)         NOT NULL DEFAULT 'BD',
    SupplierType      NVARCHAR(50)     NULL,        -- e.g. Manufacturer, Distributor, Importer
    TaxRegistrationNo NVARCHAR(100)    NULL,        -- VAT / BIN of the supplier
    PaymentTerms      NVARCHAR(80)     NULL,        -- agreed payment terms e.g. "Net30", "COD"
    LeadTimeDays      INT              NULL,         -- average days from PO to delivery
    Balance           DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- current outstanding payable balance
    Notes             NVARCHAR(1000)   NULL,
    IsActive          BIT              NOT NULL DEFAULT 1,
    CreatedAt         DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt         DATETIME2        NULL,
    UpdatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted         BIT              NOT NULL DEFAULT 0,
    RowVersion        ROWVERSION       NULL,
    CONSTRAINT PK_Suppliers PRIMARY KEY (Id),
    CONSTRAINT UX_Suppliers_Code UNIQUE (SupplierCode)
);

-- Colors: Color master used by product variants and EAV swatches.
CREATE TABLE Colors (
    Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name      NVARCHAR(80)     NOT NULL,
    HexCode   NCHAR(7)         NULL,   -- 6-digit hex with hash e.g. #FF5733
    IsActive  BIT              NOT NULL DEFAULT 1,
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt DATETIME2        NULL,
    UpdatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION      NULL,
    CONSTRAINT PK_Colors PRIMARY KEY (Id),
    CONSTRAINT UX_Colors_Name UNIQUE (Name)
);

-- Units: Unit-of-measure master with optional base-unit conversion.
-- 3NF: ConversionFactor depends on the (Id, BaseUnitId) relationship,
--      which is captured by the two columns on the same row without
--      transitivity — the factor is intrinsic to this unit's definition.
CREATE TABLE Units (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name             NVARCHAR(50)     NOT NULL,   -- full name e.g. "Kilogram"
    ShortName        NVARCHAR(20)     NOT NULL,   -- abbreviated form e.g. "kg"
    Description      NVARCHAR(255)    NULL,
    BaseUnitId       UNIQUEIDENTIFIER NULL REFERENCES Units(Id),  -- e.g. "dozen" → base "piece"
    ConversionFactor DECIMAL(18,6)    NULL,   -- how many base units equal one of this unit
    IsActive         BIT              NOT NULL DEFAULT 1,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Units PRIMARY KEY (Id),
    CONSTRAINT UX_Units_Name UNIQUE (Name)
);

-- Tags: Keyword labels for products.
CREATE TABLE Tags (
    Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name      NVARCHAR(50)     NOT NULL,
    Slug      NVARCHAR(50)     NOT NULL,
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt DATETIME2        NULL,
    UpdatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION      NULL,
    CONSTRAINT PK_Tags PRIMARY KEY (Id),
    CONSTRAINT UX_Tags_Slug UNIQUE (Slug)
);

-- BrandCategories: M:N — a brand can span many categories (e.g. Samsung in
-- Electronics AND Appliances).
-- 2NF: No non-key payload; the composite PK is the entire table.
CREATE TABLE BrandCategories (
    BrandId    UNIQUEIDENTIFIER NOT NULL REFERENCES Brands    (Id) ON DELETE CASCADE,
    CategoryId UNIQUEIDENTIFIER NOT NULL REFERENCES Categories(Id) ON DELETE CASCADE,
    CONSTRAINT PK_BrandCategories PRIMARY KEY (BrandId, CategoryId)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 7  DISCOUNTS & COUPONS
-- ───────────────────────────────────────────────────────────────────────────
-- comma-separated NVARCHAR column.  That is a repeating group — a 1NF
-- violation.  It is extracted to the DiscountApplicability junction table.
-- ═══════════════════════════════════════════════════════════════════════════

-- Discounts: All promotions — coupon codes, auto-applied deals, tier rewards.
CREATE TABLE Discounts (
    Id                     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Code                   NVARCHAR(60)     NOT NULL,   -- coupon code entered by customer; unique
    Name                   NVARCHAR(100)    NOT NULL,   -- internal display name
    Description            NVARCHAR(500)    NULL,
    DiscountTypeCode       NVARCHAR(30)     NOT NULL REFERENCES DiscountTypes(TypeCode),
    DiscountValue          DECIMAL(18,2)    NOT NULL,   -- percentage or fixed amount depending on type
    MinimumOrderAmount     DECIMAL(18,2)    NULL,        -- cart must reach this subtotal to qualify
    MaximumDiscountAmount  DECIMAL(18,2)    NULL,        -- caps the savings regardless of order size
    MaximumUsageCount      INT              NULL,        -- total redemptions across all customers; NULL = unlimited
    MaximumUsagePerUser    INT              NULL,        -- redemptions per customer; NULL = unlimited
    CurrentUsageCount      INT              NOT NULL DEFAULT 0,   -- incremented on each use; avoids COUNT(*) at checkout
    AppliesTo              NVARCHAR(20)     NOT NULL DEFAULT 'ALL'
                           CONSTRAINT CK_Discounts_AppliesTo CHECK(AppliesTo IN('ALL','PRODUCT','CATEGORY','TIER')),
    TierCode       NVARCHAR(20)     NULL REFERENCES CustomerTiers(TierCode),  -- restrict to this tier only
    StartDate              DATETIME2        NOT NULL,
    EndDate                DATETIME2        NOT NULL,
    IsActive               BIT              NOT NULL DEFAULT 1,
    IsFirstOrderOnly       BIT              NOT NULL DEFAULT 0,   -- 1 = new customers only
    IsSingleUsePerCustomer BIT              NOT NULL DEFAULT 0,
    AutoApply              BIT              NOT NULL DEFAULT 0,   -- 1 = applied automatically without code entry
    RequiresMinQty         INT              NULL,        -- minimum item quantity in cart
    RequiresShipping       BIT              NOT NULL DEFAULT 0,   -- 1 = only valid for orders with shipping
    CreatedAt              DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy              UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt              DATETIME2        NULL,
    UpdatedBy              UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted              BIT              NOT NULL DEFAULT 0,
    RowVersion             ROWVERSION       NULL,
    CONSTRAINT PK_Discounts PRIMARY KEY (Id),
    CONSTRAINT UX_Discounts_Code UNIQUE (Code),
    CONSTRAINT CK_Discounts_Dates CHECK(EndDate > StartDate)
);

-- DiscountApplicability: Scope rows declaring which products or categories
-- a discount targets when AppliesTo = 'PRODUCT' or 'CATEGORY'.
-- 1NF: Replaces the CSV ApplicableCategories / single ApplicableProductId
--      columns that were scattered across the original schemas.
-- 2NF: Both ProductId and CategoryId depend fully on the PK (Id).
CREATE TABLE DiscountApplicability (
    Id         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    DiscountId UNIQUEIDENTIFIER NOT NULL REFERENCES Discounts  (Id) ON DELETE CASCADE,
    ProductId  UNIQUEIDENTIFIER NULL,   -- FK added after Products table is created
    CategoryId UNIQUEIDENTIFIER NULL REFERENCES Categories(Id),
    CONSTRAINT PK_DiscountApplicability PRIMARY KEY (Id),
    CONSTRAINT CK_DiscountApplicability_OneOf CHECK(
        (ProductId IS NOT NULL AND CategoryId IS NULL) OR
        (CategoryId IS NOT NULL AND ProductId IS NULL)
    )
);

-- DiscountUsageLog: Immutable audit trail of every coupon / discount redemption.
-- Covers both e-commerce orders and POS transactions via nullable FKs.
CREATE TABLE DiscountUsageLog (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    DiscountId       UNIQUEIDENTIFIER NOT NULL REFERENCES Discounts(Id) ON DELETE CASCADE,
    OrderId          UNIQUEIDENTIFIER NULL,   -- FK set after Orders
    PosTransactionId UNIQUEIDENTIFIER NULL,   -- FK set after PosTransactions
    UserId           UNIQUEIDENTIFIER    NOT NULL REFERENCES Users(Id),
    CustomerId       UNIQUEIDENTIFIER NULL,   -- FK set after Customers
    DiscountAmount   DECIMAL(18,2)    NOT NULL,   -- actual monetary saving applied
    UsedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_DiscountUsageLog PRIMARY KEY (Id),
    CONSTRAINT CK_DiscountUsages_Channel CHECK(OrderId IS NOT NULL OR PosTransactionId IS NOT NULL)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 8  EAV VARIANT ATTRIBUTE SYSTEM
-- ───────────────────────────────────────────────────────────────────────────
-- Used ONLY for variant-generating attribute axes (Color, Size, RAM, Storage).
-- Descriptive display attributes (Material, Connectivity) use the flat
-- ProductSpecifications table in Section 9 — no EAV there.
-- ═══════════════════════════════════════════════════════════════════════════

-- AttributeTypes: Defines one variant-generating axis.
-- AffectsImage = 1 tells the front-end to switch the gallery when this
-- attribute changes, without hardcoded "if color" logic in the application.
CREATE TABLE AttributeTypes (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name         NVARCHAR(80)     NOT NULL,
    Slug         NVARCHAR(90)     NOT NULL,
    UiType       NVARCHAR(20)     NOT NULL DEFAULT 'Dropdown'
                 CONSTRAINT CK_AttrTypes_UI CHECK(UiType IN('Swatch','Dropdown','Button','Text')),
    AffectsPrice BIT              NOT NULL DEFAULT 0,   -- 1 = selecting this axis changes the price
    AffectsSku   BIT              NOT NULL DEFAULT 1,   -- 1 = contributes to the variant SKU suffix
    AffectsImage BIT              NOT NULL DEFAULT 0,   -- 1 = gallery switches when value changes
    AffectsStock BIT              NOT NULL DEFAULT 1,   -- 1 = stock tracked separately per variant
    IsFilterable BIT              NOT NULL DEFAULT 1,   -- 1 = shown in sidebar filter panel
    SortOrder    INT              NOT NULL DEFAULT 0,
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_AttributeTypes PRIMARY KEY (Id),
    CONSTRAINT UX_AttributeTypes_Name UNIQUE (Name),
    CONSTRAINT UX_AttributeTypes_Slug UNIQUE (Slug)
);

-- AttributeOptions: A single selectable value for one attribute type axis.
-- ColorId links to the Colors master so HEX swatches are rendered consistently.
CREATE TABLE AttributeOptions (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    AttributeTypeId UNIQUEIDENTIFIER NOT NULL REFERENCES AttributeTypes(Id) ON DELETE CASCADE,
    ColorId         UNIQUEIDENTIFIER NULL REFERENCES Colors(Id),   -- populated for swatch-type attributes
    Value           NVARCHAR(120)    NOT NULL,   -- raw value e.g. "Red", "XL", "16GB"
    DisplayValue    NVARCHAR(120)    NULL,        -- optional override label shown in UI
    SortOrder       INT              NOT NULL DEFAULT 0,
    IsActive        BIT              NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_AttributeOptions PRIMARY KEY (Id),
    CONSTRAINT UX_AttributeOptions UNIQUE (AttributeTypeId, Value)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 9  PRODUCTS
-- ═══════════════════════════════════════════════════════════════════════════

-- Products: Core product entity.  ProductType discriminates Simple, Variant,
-- FixedBundle, DynamicBundle, and Service products.
-- 1NF: RatingAverage and ReviewCount were stored columns in the original
--      schema — they derive from the Reviews table and are therefore a 1NF
--      violation (stored computation).  Removed; see vw_ProductStats.
-- 3NF: DiscountPercent was a product-level column but discounts depend on
--      promotional rules, not on the product itself.  Removed; see Discounts.
--      SupplierId single FK replaced by ProductSupplierLinks junction because
--      a product can be sourced from multiple suppliers.
CREATE TABLE Products (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CategoryId       UNIQUEIDENTIFIER NOT NULL REFERENCES Categories      (Id),
    BrandId          UNIQUEIDENTIFIER NULL     REFERENCES Brands          (Id) ON DELETE SET NULL,
    ColorId          UNIQUEIDENTIFIER NULL     REFERENCES Colors          (Id),
    UnitId           UNIQUEIDENTIFIER NULL     REFERENCES Units           (Id),
    TaxRateId        UNIQUEIDENTIFIER NULL     REFERENCES TaxRates        (Id),
    ConditionCode    NVARCHAR(20)     NULL     REFERENCES ProductConditions(ConditionCode),
    SellerId         UNIQUEIDENTIFIER NULL,   -- FK to Sellers; added after Sellers table
    -- Identifiers
    ProductCode      NVARCHAR(50)     NULL,   -- internal stock-keeping code
    Name             NVARCHAR(200)    NOT NULL,
    Slug             NVARCHAR(200)    NOT NULL,   -- URL-safe segment e.g. "samsung-galaxy-s25"
    ShortName        NVARCHAR(100)    NULL,        -- abbreviated name for receipts / POS display
    SKU              NVARCHAR(50)     NULL,        -- global stock-keeping unit
    Barcode          NVARCHAR(100)    NULL,        -- EAN / UPC barcode
    -- Content
    ShortDescription NVARCHAR(500)    NULL,        -- one-line summary shown in listing cards
    Description      NVARCHAR(MAX)    NULL,        -- full rich-text product description
    -- Type discriminator
    ProductType      NVARCHAR(20)     NOT NULL DEFAULT 'Simple'
                     CONSTRAINT CK_Products_Type CHECK(ProductType IN('Simple','Variant','FixedBundle','DynamicBundle','Service')),
    -- Pricing
    CostPrice        DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- landed cost (used for margin reports)
    SalePrice        DECIMAL(18,2)    NOT NULL CHECK(SalePrice >= 0),
    OriginalPrice    DECIMAL(18,2)    NULL,        -- "was" price shown crossed out in UI
    IsTaxInclusive   BIT              NOT NULL DEFAULT 0,   -- 1 = SalePrice already includes tax
    -- Physical attributes
    WeightKg         DECIMAL(8,3)     NULL,        -- gross weight used for shipping cost calculation
    Dimensions       NVARCHAR(100)    NULL,        -- "L × W × H cm" free-text string
    ShelfLocation    NVARCHAR(100)    NULL,        -- physical bin reference e.g. "A3-R2-S4"
    -- StockItems thresholds
    MinimumStockLevel INT             NOT NULL DEFAULT 0,   -- triggers low-stock alert
    ReorderLevel     DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- triggers auto-reorder
    MinSaleQty       DECIMAL(18,2)    NOT NULL DEFAULT 1,   -- smallest quantity a customer may purchase
    MaxSaleQty       DECIMAL(18,2)    NULL,                 -- largest quantity; NULL = unlimited
    -- Feature flags
    IsFeatured       BIT              NOT NULL DEFAULT 0,
    IsBestSeller     BIT              NOT NULL DEFAULT 0,
    IsNewArrival     BIT              NOT NULL DEFAULT 0,
    IsPerishable     BIT              NOT NULL DEFAULT 0,   -- 1 = requires batch / expiry tracking
    HasExpiry        BIT              NOT NULL DEFAULT 0,   -- 1 = must display expiry date
    IsActive         BIT              NOT NULL DEFAULT 1,
    -- Analytics (ViewCount is a deliberately accepted denorm — updated asynchronously)
    ViewCount        INT              NOT NULL DEFAULT 0,
    -- SEO
    MetaTitle        NVARCHAR(200)    NULL,   -- overrides <title> on product page
    MetaDescription  NVARCHAR(500)    NULL,   -- <meta name="description"> content
    -- Audit
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Products PRIMARY KEY (Id),
    CONSTRAINT UX_Products_Slug UNIQUE (Slug)
);
CREATE UNIQUE INDEX UX_Products_SKU     ON Products(SKU)     WHERE SKU     IS NOT NULL AND IsDeleted=0;
CREATE UNIQUE INDEX UX_Products_Barcode ON Products(Barcode) WHERE Barcode IS NOT NULL AND IsDeleted=0;
CREATE INDEX IX_Products_Category ON Products(CategoryId) WHERE IsDeleted=0;
CREATE INDEX IX_Products_Brand    ON Products(BrandId)    WHERE BrandId IS NOT NULL AND IsDeleted=0;
CREATE INDEX IX_Products_Active   ON Products(IsActive, ProductType) WHERE IsDeleted=0;

-- ProductSupplierLinks: One product can be sourced from multiple suppliers.
-- 3NF: IsPreferred depends on the (ProductId, SupplierId) pair — the
--      relationship itself — not on either FK column alone.
CREATE TABLE ProductSupplierLinks (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId    UNIQUEIDENTIFIER NOT NULL REFERENCES Products (Id) ON DELETE CASCADE,
    SupplierId   UNIQUEIDENTIFIER NOT NULL REFERENCES Suppliers(Id) ON DELETE CASCADE,
    SupplierSKU  NVARCHAR(50)     NULL,   -- supplier's own item reference code
    UnitCost     DECIMAL(18,2)    NULL,   -- supplier-specific unit cost
    LeadTimeDays INT              NULL,   -- lead time specific to this supplier–product pair
    IsPreferred  BIT              NOT NULL DEFAULT 0,   -- 1 = default vendor for re-orders
    IsActive     BIT              NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_ProductSuppliers PRIMARY KEY (Id),
    CONSTRAINT UX_ProductSupplierLinks UNIQUE (ProductId, SupplierId)
);

-- ProductPriceHistories: Immutable audit trail of price changes.
CREATE TABLE ProductPriceHistories (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId       UNIQUEIDENTIFIER NOT NULL REFERENCES Products(Id),
    ChangedByUserId UNIQUEIDENTIFIER    NOT NULL REFERENCES Users   (Id),
    OldCostPrice    DECIMAL(18,2)    NOT NULL,
    OldSalePrice    DECIMAL(18,2)    NOT NULL,
    NewCostPrice    DECIMAL(18,2)    NOT NULL,
    NewSalePrice    DECIMAL(18,2)    NOT NULL,
    EffectiveFrom   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    EffectiveTo     DATETIME2        NULL,   -- NULL = this is the currently active price
    Reason          NVARCHAR(255)    NULL,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_ProductPriceHistories PRIMARY KEY (Id)
);
CREATE INDEX IX_ProductPriceHistories ON ProductPriceHistories(ProductId, EffectiveFrom DESC);

-- ProductBatches: Batch / lot tracking for perishable and expiry-dated items.
CREATE TABLE ProductBatches (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId       UNIQUEIDENTIFIER NOT NULL REFERENCES Products(Id),
    BatchNo         NVARCHAR(100)    NOT NULL,   -- lot / batch number from manufacturer
    ManufactureDate DATETIME2        NULL,
    ExpiryDate      DATETIME2        NULL,
    PurchasePrice   DECIMAL(18,2)    NOT NULL DEFAULT 0,
    SalePrice       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_ProductBatches PRIMARY KEY (Id),
    CONSTRAINT UX_ProductBatches UNIQUE (ProductId, BatchNo)
);

-- ProductVariants: Sellable attribute combinations (e.g. Red / XL / 128 GB).
-- StockQuantity is intentionally absent — inventory is tracked at warehouse
-- level in StockItems, which is the single source of truth.
CREATE TABLE ProductVariants (
    Id            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId     UNIQUEIDENTIFIER NOT NULL REFERENCES Products(Id) ON DELETE CASCADE,
    Name          NVARCHAR(100)    NOT NULL,   -- auto-generated label e.g. "Red / XL"
    SKU           NVARCHAR(50)     NULL,
    Barcode       NVARCHAR(50)     NULL,
    CostPrice     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    PriceModifier DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- added to Product.SalePrice; 0 = same price
    OverridePrice DECIMAL(18,2)    NULL,                 -- if set, PriceModifier is ignored
    WeightKg      DECIMAL(8,3)     NULL,        -- per-variant weight override for shipping
    IsDefault     BIT              NOT NULL DEFAULT 0,   -- 1 = pre-selected on product page
    IsActive      BIT              NOT NULL DEFAULT 1,
    SortOrder     INT              NOT NULL DEFAULT 0,
    ImageUrl      NVARCHAR(500)    NULL,   -- quick thumbnail URL; full gallery via ProductMedia
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt     DATETIME2        NULL,
    UpdatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted     BIT              NOT NULL DEFAULT 0,
    RowVersion    ROWVERSION       NULL,
    CONSTRAINT PK_ProductVariants PRIMARY KEY (Id)
);
CREATE UNIQUE INDEX UX_ProductVariants_SKU     ON ProductVariants(SKU)     WHERE SKU     IS NOT NULL AND IsDeleted=0;
CREATE UNIQUE INDEX UX_ProductVariants_Barcode ON ProductVariants(Barcode) WHERE Barcode IS NOT NULL AND IsDeleted=0;
CREATE INDEX IX_Variants_Product ON ProductVariants(ProductId) WHERE IsDeleted=0;

-- ProductAttributeLinks: Declares which attribute axes apply to a product.
-- 2NF: IsRequired and SortOrder depend on (ProductId, AttributeTypeId) fully.
CREATE TABLE ProductAttributeLinks (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId       UNIQUEIDENTIFIER NOT NULL REFERENCES Products      (Id) ON DELETE CASCADE,
    AttributeTypeId UNIQUEIDENTIFIER NOT NULL REFERENCES AttributeTypes(Id),
    IsRequired      BIT              NOT NULL DEFAULT 1,
    SortOrder       INT              NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_ProductAttributeLinks PRIMARY KEY (Id),
    CONSTRAINT UX_ProductAttributeLinks UNIQUE (ProductId, AttributeTypeId)
);

-- VariantAttributeOptions: Assigns specific attribute option values to a variant.
-- 2NF: Pure junction — the composite (VariantId, OptionId) is the natural key.
CREATE TABLE VariantAttributeOptions (
    Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    VariantId UNIQUEIDENTIFIER NOT NULL REFERENCES ProductVariants (Id) ON DELETE CASCADE,
    OptionId  UNIQUEIDENTIFIER NOT NULL REFERENCES AttributeOptions(Id),
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION      NULL,
    CONSTRAINT PK_VariantAttributeOptions PRIMARY KEY (Id),
    CONSTRAINT UX_VariantAttributeOptions UNIQUE (VariantId, OptionId)
);

-- VariantAttributeMatrix: Pre-computed attribute-combination → variant look-up index.
-- AttributeCombination stores a canonical sorted JSON object
-- e.g. {"color":"<guid>","size":"<guid>"} to enable O(1) cart look-up.
-- Rebuilt by the application service whenever variants change.
CREATE TABLE VariantAttributeMatrix (
    Id                   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId            UNIQUEIDENTIFIER NOT NULL REFERENCES Products       (Id),
    VariantId            UNIQUEIDENTIFIER NOT NULL REFERENCES ProductVariants(Id) ON DELETE CASCADE,
    AttributeCombination NVARCHAR(MAX)    NOT NULL,   -- canonical JSON, used as a look-up key
    IsAvailable          BIT              NOT NULL DEFAULT 1,   -- 0 = out-of-stock combination
    CreatedAt            DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted            BIT              NOT NULL DEFAULT 0,
    RowVersion           ROWVERSION       NULL,
    CONSTRAINT PK_VariantAttributeMatrix PRIMARY KEY (Id),
    CONSTRAINT UX_VariantAttributeMatrix UNIQUE (ProductId, VariantId)
);

-- ProductSpecifications: Flat descriptive specifications shown on the detail page.
-- These are display-only and do NOT drive variant generation (EAV handles that).
CREATE TABLE ProductSpecifications (
    Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    SpecName  NVARCHAR(100)    NOT NULL,   -- e.g. "Display Resolution", "Battery Capacity"
    SortOrder INT              NOT NULL DEFAULT 0,
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION      NULL,
    CONSTRAINT PK_Specs PRIMARY KEY (Id),
    CONSTRAINT UX_ProductSpecifications UNIQUE (SpecName)
);

-- ProductSpecificationValues: Per-product values for each spec.
-- VariantId allows variant-level overrides (e.g. storage capacity varies per SKU).
CREATE TABLE ProductSpecificationValues (
    Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId UNIQUEIDENTIFIER NOT NULL REFERENCES Products            (Id) ON DELETE CASCADE,
    VariantId UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants     (Id),
    SpecId    UNIQUEIDENTIFIER NOT NULL REFERENCES ProductSpecifications(Id) ON DELETE CASCADE,
    Value     NVARCHAR(MAX)    NOT NULL,   -- free-text value e.g. "1080 × 2400 px"
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt DATETIME2        NULL,
    UpdatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION      NULL,
    CONSTRAINT PK_ProductSpecificationValues PRIMARY KEY (Id)
);

-- ProductImages: URL-referenced display images for products and variants.
CREATE TABLE ProductImages (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId    UNIQUEIDENTIFIER NOT NULL REFERENCES Products       (Id) ON DELETE CASCADE,
    VariantId    UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants(Id),  -- NULL = shown for all variants
    ImageUrl     NVARCHAR(500)    NOT NULL,   -- CDN or local URL
    AltText      NVARCHAR(200)    NULL,        -- HTML alt attribute for accessibility and SEO
    SortOrder    INT              NOT NULL DEFAULT 0,
    IsPrimary    BIT              NOT NULL DEFAULT 0,   -- 1 = main image shown first
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_ProductImages PRIMARY KEY (Id)
);
CREATE INDEX IX_ProductImages_Product ON ProductImages(ProductId) WHERE IsDeleted=0;

-- ProductTaxRates: M:N between products and tax rates (stacked taxes).
-- 2NF: IsActive depends on (ProductId, TaxRateId) — both parts of the key.
CREATE TABLE ProductTaxRates (
    Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId UNIQUEIDENTIFIER NOT NULL REFERENCES Products(Id) ON DELETE CASCADE,
    TaxRateId UNIQUEIDENTIFIER NOT NULL REFERENCES TaxRates(Id),
    IsActive  BIT              NOT NULL DEFAULT 1,
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt DATETIME2        NULL,
    UpdatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION      NULL,
    CONSTRAINT PK_ProductTax PRIMARY KEY (Id),
    CONSTRAINT UX_ProductTaxes UNIQUE (ProductId, TaxRateId)
);

-- ProductTags: M:N junction — products to keyword tags.
-- 2NF: No non-key payload; composite PK is the full table.
CREATE TABLE ProductTags (
    ProductId UNIQUEIDENTIFIER NOT NULL REFERENCES Products(Id) ON DELETE CASCADE,
    TagId     UNIQUEIDENTIFIER NOT NULL REFERENCES Tags    (Id) ON DELETE CASCADE,
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CONSTRAINT PK_ProductTags PRIMARY KEY (ProductId, TagId)
);

-- Now add FK for DiscountApplicability.ProductId (Products table now exists)
ALTER TABLE DiscountApplicability
    ADD CONSTRAINT FK_DiscountApplicability_Products
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 10  MEDIA / BLOB STORAGE
-- ───────────────────────────────────────────────────────────────────────────
-- Binary bytes are stored separately from metadata so that metadata-only
-- queries never accidentally load megabytes of image data.
-- ETag is NCHAR(32) because an MD5 hash is always exactly 32 hex characters.
-- ═══════════════════════════════════════════════════════════════════════════

-- MediaAssets: Central repository for all non-product files (blog, CMS, avatars).
CREATE TABLE MediaAssets (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    FileName        NVARCHAR(260)    NOT NULL,
    FilePath        NVARCHAR(MAX)    NOT NULL,   -- relative path or CDN URL
    ContentType     NVARCHAR(80)     NOT NULL,   -- MIME type e.g. image/jpeg
    FileSizeBytes   BIGINT           NOT NULL CHECK(FileSizeBytes > 0),
    OriginalName    NVARCHAR(260)    NULL,        -- original filename before server rename
    AltText         NVARCHAR(200)    NULL,
    Width           INT              NULL,        -- pixels (images / video)
    Height          INT              NULL,
    DurationSeconds INT              NULL,        -- seconds (video / audio assets)
    StorageProvider NVARCHAR(20)     NOT NULL DEFAULT 'Local'
                    CONSTRAINT CK_MediaAssets_Provider CHECK(StorageProvider IN('Local','S3','AzureBlob','GCS')),
    UploadedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_MediaAssets PRIMARY KEY (Id)
);

-- ProductMedia: Metadata record for a binary product image stored in ProductMediaBlob.
CREATE TABLE ProductMedia (
    Id            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId     UNIQUEIDENTIFIER NOT NULL REFERENCES Products       (Id) ON DELETE CASCADE,
    VariantId     UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants(Id),
    Scope         NVARCHAR(20)     NOT NULL DEFAULT 'Product'
                  CONSTRAINT CK_ProductMedia_Scope CHECK(Scope IN('Product','Variant','Bundle','Component')),
    MediaType     NVARCHAR(10)     NOT NULL DEFAULT 'Image'
                  CONSTRAINT CK_ProductMedia_Type CHECK(MediaType IN('Image','Video','Document')),
    FileName      NVARCHAR(260)    NOT NULL,
    MimeType      NVARCHAR(80)     NOT NULL,
    FileSizeBytes INT              NOT NULL CHECK(FileSizeBytes > 0),
    WidthPx       INT              NULL,
    HeightPx      INT              NULL,
    AltText       NVARCHAR(200)    NULL,
    IsPrimary     BIT              NOT NULL DEFAULT 0,
    SortOrder     INT              NOT NULL DEFAULT 0,
    ETag          NCHAR(32)        NULL,   -- MD5 hex hash (always 32 chars) for HTTP cache validation
    UploadedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt     DATETIME2        NULL,
    UpdatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted     BIT              NOT NULL DEFAULT 0,
    RowVersion    ROWVERSION       NULL,
    CONSTRAINT PK_Media PRIMARY KEY (Id)
);
CREATE INDEX IX_Media ON ProductMedia(ProductId, Scope, SortOrder) WHERE IsDeleted=0;

-- ProductMediaBlob: Binary bytes only.  NEVER SELECT * from this table.
-- Always project only the column you need (Data, ThumbnailData, or WebpData).
CREATE TABLE ProductMediaBlob (
    MediaId       UNIQUEIDENTIFIER NOT NULL,
    Data          VARBINARY(MAX)   NOT NULL,   -- original file bytes
    ThumbnailData VARBINARY(MAX)   NULL,        -- 150 × 150 px resized thumbnail
    WebpData      VARBINARY(MAX)   NULL,        -- WebP-converted version for modern browsers
    CONSTRAINT PK_ProductMediaBlob PRIMARY KEY (MediaId),
    CONSTRAINT FK_ProductMediaBlob FOREIGN KEY (MediaId) REFERENCES ProductMedia(Id) ON DELETE CASCADE
);

-- AttributeOptionMedia: Swatch / gallery images per attribute option per product.
CREATE TABLE AttributeOptionMedia (
    Id            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OptionId      UNIQUEIDENTIFIER NOT NULL REFERENCES AttributeOptions(Id) ON DELETE CASCADE,
    ProductId     UNIQUEIDENTIFIER NOT NULL REFERENCES Products        (Id) ON DELETE CASCADE,
    FileName      NVARCHAR(260)    NOT NULL,
    MimeType      NVARCHAR(80)     NOT NULL,
    FileSizeBytes INT              NOT NULL,
    WidthPx       INT              NULL,
    HeightPx      INT              NULL,
    AltText       NVARCHAR(200)    NULL,
    IsPrimary     BIT              NOT NULL DEFAULT 0,
    SortOrder     INT              NOT NULL DEFAULT 0,
    ETag          NCHAR(32)        NULL,
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt     DATETIME2        NULL,
    UpdatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted     BIT              NOT NULL DEFAULT 0,
    RowVersion    ROWVERSION       NULL,
    CONSTRAINT PK_AttributeOptionMedia PRIMARY KEY (Id)
);
CREATE TABLE AttributeOptionMediaBlob (
    MediaId    UNIQUEIDENTIFIER NOT NULL,
    Data       VARBINARY(MAX)   NOT NULL,
    SwatchData VARBINARY(MAX)   NULL,   -- 32 × 32 px colour swatch
    WebpData   VARBINARY(MAX)   NULL,
    CONSTRAINT PK_AttributeOptionMediaBlob PRIMARY KEY (MediaId),
    CONSTRAINT FK_AttributeOptionMediaBlob FOREIGN KEY (MediaId) REFERENCES AttributeOptionMedia(Id) ON DELETE CASCADE
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 11  BUNDLE / COMBO PRODUCTS
-- ═══════════════════════════════════════════════════════════════════════════

-- BundleComponents: Fixed component variants in a FixedBundle product.
-- Bundle inventory is NOT tracked here; deducted from components at sale time.
CREATE TABLE BundleComponents (
    Id                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    BundleProductId    UNIQUEIDENTIFIER NOT NULL REFERENCES Products       (Id),
    ComponentVariantId UNIQUEIDENTIFIER NOT NULL REFERENCES ProductVariants(Id),
    Quantity           DECIMAL(18,2)    NOT NULL DEFAULT 1 CHECK(Quantity > 0),
    IsSubstitutable    BIT              NOT NULL DEFAULT 0,   -- 1 = customer may swap for another variant
    SortOrder          INT              NOT NULL DEFAULT 0,
    CreatedAt          DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy          UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt          DATETIME2        NULL,
    UpdatedBy          UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted          BIT              NOT NULL DEFAULT 0,
    RowVersion         ROWVERSION       NULL,
    CONSTRAINT PK_BundleComponents PRIMARY KEY (Id),
    CONSTRAINT UX_BundleComponents UNIQUE (BundleProductId, ComponentVariantId)
);

-- BundleOptionGroups: A named choice group in a DynamicBundle.
CREATE TABLE BundleOptionGroups (
    Id                   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    BundleProductId      UNIQUEIDENTIFIER NOT NULL REFERENCES Products(Id) ON DELETE CASCADE,
    GroupName            NVARCHAR(100)    NOT NULL,
    IsRequired           BIT              NOT NULL DEFAULT 1,
    MinSelections        INT              NOT NULL DEFAULT 1,
    MaxSelections        INT              NOT NULL DEFAULT 1,
    QuantityPerSelection INT              NOT NULL DEFAULT 1,
    SortOrder            INT              NOT NULL DEFAULT 0,
    CreatedAt            DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt            DATETIME2        NULL,
    UpdatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted            BIT              NOT NULL DEFAULT 0,
    RowVersion           ROWVERSION       NULL,
    CONSTRAINT PK_BundleOptionGroups PRIMARY KEY (Id)
);

-- BundleOptionItems: Variant choices available within a bundle group.
CREATE TABLE BundleOptionItems (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    GroupId         UNIQUEIDENTIFIER NOT NULL REFERENCES BundleOptionGroups(Id) ON DELETE CASCADE,
    VariantId       UNIQUEIDENTIFIER NOT NULL REFERENCES ProductVariants    (Id),
    PriceAdjustment DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- upsell / downsell offset from group base
    IsDefault       BIT              NOT NULL DEFAULT 0,
    SortOrder       INT              NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_BundleOptionItems PRIMARY KEY (Id),
    CONSTRAINT UX_BundleOptionItems UNIQUE (GroupId, VariantId)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 12  COLLECTIONS, PRICE LISTS & FLASH DEALS
-- ═══════════════════════════════════════════════════════════════════════════

-- ProductCollections: Curated marketing groupings of products.
CREATE TABLE ProductCollections (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name           NVARCHAR(150)    NOT NULL,
    Slug           NVARCHAR(150)    NOT NULL,
    Description    NVARCHAR(MAX)    NULL,
    ImageUrl       NVARCHAR(MAX)    NULL,
    DisplayOrder   INT              NOT NULL DEFAULT 0,
    IsActive       BIT              NOT NULL DEFAULT 1,
    ShowInHomePage BIT              NOT NULL DEFAULT 0,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt      DATETIME2        NULL,
    UpdatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_Collections PRIMARY KEY (Id),
    CONSTRAINT UX_Collections_Slug UNIQUE (Slug)
);

-- ProductCollectionItems: M:N junction with display ordering.
CREATE TABLE ProductCollectionItems (
    Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductCollectionId UNIQUEIDENTIFIER NOT NULL REFERENCES ProductCollections(Id) ON DELETE CASCADE,
    ProductId           UNIQUEIDENTIFIER NOT NULL REFERENCES Products          (Id) ON DELETE CASCADE,
    DisplayOrder        INT              NOT NULL DEFAULT 0,
    CreatedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    RowVersion          ROWVERSION       NULL,
    CONSTRAINT PK_ProductCollectionItems PRIMARY KEY (Id),
    CONSTRAINT UX_ProductCollectionItems UNIQUE (ProductCollectionId, ProductId)
);

-- PriceLists: Named pricing tiers for B2B and loyalty customer groups.
CREATE TABLE PriceLists (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name             NVARCHAR(150)    NOT NULL,
    PriceListType    NVARCHAR(30)     NOT NULL DEFAULT 'CustomerGroup'
                     CONSTRAINT CK_PriceLists_Type CHECK(PriceListType IN('Retail','Wholesale','CustomerGroup','Promotional')),
    TierCode NVARCHAR(20)     NULL REFERENCES CustomerTiers(TierCode),   -- restrict to this tier
    StartDate        DATETIME2        NOT NULL,
    EndDate          DATETIME2        NULL,
    IsActive         BIT              NOT NULL DEFAULT 1,
    Description      NVARCHAR(MAX)    NULL,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_PriceLists PRIMARY KEY (Id)
);

-- PriceListItems: Per-product price overrides within a price list.
CREATE TABLE PriceListItems (
    Id            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PriceListId   UNIQUEIDENTIFIER NOT NULL REFERENCES PriceLists    (Id) ON DELETE CASCADE,
    ProductId     UNIQUEIDENTIFIER NOT NULL REFERENCES Products      (Id) ON DELETE CASCADE,
    VariantId     UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants(Id),
    SalePrice     DECIMAL(18,2)    NOT NULL,   -- price override for this customer tier
    MinQuantity   DECIMAL(18,2)    NULL,        -- tiered pricing lower bound
    MaxQuantity   DECIMAL(18,2)    NULL,        -- tiered pricing upper bound; NULL = no cap
    EffectiveDate DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt     DATETIME2        NULL,
    UpdatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted     BIT              NOT NULL DEFAULT 0,
    RowVersion    ROWVERSION       NULL,
    CONSTRAINT PK_PriceListItems PRIMARY KEY (Id)
);

-- FlashDeals: Time-boxed promotional sale events.
CREATE TABLE FlashDeals (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Title          NVARCHAR(200)    NOT NULL,
    Slug           NVARCHAR(200)    NOT NULL,
    Description    NVARCHAR(MAX)    NULL,
    ImageUrl       NVARCHAR(MAX)    NULL,
    StartDate      DATETIME2        NOT NULL,
    EndDate        DATETIME2        NOT NULL,
    IsActive       BIT              NOT NULL DEFAULT 1,
    ShowInHomePage BIT              NOT NULL DEFAULT 0,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt      DATETIME2        NULL,
    UpdatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_Deals PRIMARY KEY (Id),
    CONSTRAINT CK_FlashDeals_Dates CHECK(EndDate > StartDate)
);

-- FlashDealProducts: Individual product entries in a flash deal.
CREATE TABLE FlashDealProducts (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    FlashDealId     UNIQUEIDENTIFIER NOT NULL REFERENCES FlashDeals(Id) ON DELETE CASCADE,
    ProductId       UNIQUEIDENTIFIER NOT NULL REFERENCES Products  (Id) ON DELETE CASCADE,
    DiscountPercent DECIMAL(5,2)     NOT NULL DEFAULT 0,   -- percentage off for this product in this deal
    DiscountAmount  DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- fixed amount off (use one or the other)
    DiscountType    TINYINT          NOT NULL DEFAULT 1,   -- 1 = percentage; 2 = fixed amount
    MaxQuantity     INT              NOT NULL DEFAULT 0,   -- maximum units available at deal price; 0 = unlimited
    SoldQuantity    INT              NOT NULL DEFAULT 0,   -- running count; incremented on each purchase
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_FlashDealProducts PRIMARY KEY (Id),
    CONSTRAINT UX_FlashDealProducts UNIQUE (FlashDealId, ProductId)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 13  CUSTOMERS, SELLERS & LOYALTY
-- ───────────────────────────────────────────────────────────────────────────
-- 1NF: TotalOrders, TotalPurchases, TotalSpent, OrderCount, LastOrderDate
--      were stored on Customers in the original schemas.  These are all
--      computed values derivable from the Orders table — a 1NF violation
--      (stored computation).  Removed; see vw_CustomerStats.
-- 3NF: CustomerProfiles.LifetimeSpend depended on the Orders table via
--      Customers — a transitive dependency.  Removed.
-- ═══════════════════════════════════════════════════════════════════════════

-- Customers: Domain-level profile linked 1:1 to the identity Users table.
CREATE TABLE Customers (
    Id                   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    UserId               UNIQUEIDENTIFIER    NULL REFERENCES Users(Id) ON DELETE SET NULL,
    CustomerCode         NVARCHAR(50)     NOT NULL,   -- human-readable ID e.g. CUS-00042
    CustomerType         NVARCHAR(30)     NOT NULL DEFAULT 'Retail'
                         CONSTRAINT CK_Customers_Type CHECK(CustomerType IN('Retail','Wholesale','Corporate','VIP')),
    Phone                NVARCHAR(30)     NULL,
    AlternatePhone       NVARCHAR(30)     NULL,
    Email                NVARCHAR(256)    NULL,
    DateOfBirth          DATETIME2        NULL,
    Gender               NVARCHAR(10)     NULL,
    CompanyName          NVARCHAR(200)    NULL,
    TaxNumber            NVARCHAR(50)     NULL,        -- customer's VAT / TIN registration
    AddressLine1         NVARCHAR(200)    NULL,        -- primary address snapshot for POS quick-lookup
    City                 NVARCHAR(100)    NULL,
    Country              NCHAR(2)         NOT NULL DEFAULT 'BD',
    Balance              DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- outstanding account receivable
    CreditLimit          DECIMAL(18,2)    NULL,                 -- maximum credit balance permitted
    LoyaltyPoints        INT              NOT NULL DEFAULT 0,   -- current redeemable point balance
    CustomerGroup        NVARCHAR(50)     NULL,        -- optional free-text segment tag
    ReferralCode         NVARCHAR(20)     NULL,        -- this customer's unique referral link code
    ReferredByCustomerId UNIQUEIDENTIFIER NULL REFERENCES Customers(Id),   -- who referred this customer
    RegistrationDate     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    LastPurchaseDate     DATETIME2        NULL,        -- updated by service after each order (accepted denorm)
    IsActive             BIT              NOT NULL DEFAULT 1,
    Notes                NVARCHAR(1000)   NULL,
    CreatedAt            DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt            DATETIME2        NULL,
    UpdatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted            BIT              NOT NULL DEFAULT 0,
    RowVersion           ROWVERSION       NULL,
    CONSTRAINT PK_Customers PRIMARY KEY (Id),
    CONSTRAINT UX_Customers_Code UNIQUE (CustomerCode)
);
CREATE UNIQUE INDEX UX_Customers_UserId       ON Customers(UserId)       WHERE UserId IS NOT NULL;
CREATE UNIQUE INDEX UX_Customers_ReferralCode ON Customers(ReferralCode) WHERE ReferralCode IS NOT NULL;

-- CustomerProfiles: Loyalty tier assignment and marketing preferences.
CREATE TABLE CustomerProfiles (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CustomerId      UNIQUEIDENTIFIER NOT NULL REFERENCES Customers   (Id) ON DELETE CASCADE,
    TierCode        NVARCHAR(20)     NOT NULL REFERENCES CustomerTiers(TierCode),
    NewsletterOptIn BIT              NOT NULL DEFAULT 0,
    SmsOptIn        BIT              NOT NULL DEFAULT 0,   -- SMS marketing consent
    TierUpgradeDate DATETIME2        NULL,   -- date the customer last moved up a tier
    TierReviewDate  DATETIME2        NULL,   -- next scheduled tier eligibility check
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_CustomerProfiles PRIMARY KEY (Id),
    CONSTRAINT UX_CustomerProfiles UNIQUE (CustomerId)
);

-- LoyaltyTransactions: Full loyalty point ledger.
-- The balance is NEVER stored directly; it is always SUM(Points) from this table.
-- 1NF: Replaces the simple LoyaltyPoints INT on Customers, which was a
--      stored aggregation of this ledger.
CREATE TABLE LoyaltyTransactions (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CustomerId      UNIQUEIDENTIFIER NOT NULL REFERENCES Customers(Id) ON DELETE CASCADE,
    OrderId         UNIQUEIDENTIFIER NULL,   -- FK set after Orders
    PosTransId      UNIQUEIDENTIFIER NULL,   -- FK set after PosTransactions
    TransactionType NVARCHAR(20)     NOT NULL
                    CONSTRAINT CK_LoyaltyTx_Type CHECK(TransactionType IN('Earn','Redeem','Expire','Adjust','Bonus')),
    Points          INT              NOT NULL,   -- positive = credit; negative = debit
    Description     NVARCHAR(MAX)    NULL,
    TransactionDate DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    ExpiryDate      DATETIME2        NULL,   -- when this batch of earned points expires
    IsUsed          BIT              NOT NULL DEFAULT 0,   -- 1 = these points have been redeemed
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_LoyaltyTransactions PRIMARY KEY (Id)
);
CREATE INDEX IX_LoyaltyTransactions ON LoyaltyTransactions(CustomerId, TransactionDate DESC);

-- Sellers: Marketplace vendor profiles.
CREATE TABLE Sellers (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    UserId           UNIQUEIDENTIFIER    NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    StoreName        NVARCHAR(200)    NOT NULL,
    Slug             NVARCHAR(200)    NOT NULL,
    StoreDescription NVARCHAR(MAX)    NULL,
    StoreLogo        NVARCHAR(500)    NULL,
    StoreBanner      NVARCHAR(500)    NULL,
    Email            NVARCHAR(256)    NULL,
    Phone            NVARCHAR(30)     NULL,
    AddressLine1     NVARCHAR(200)    NULL,
    City             NVARCHAR(100)    NULL,
    Country          NCHAR(2)         NOT NULL DEFAULT 'BD',
    Balance          DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- accrued payout balance
    CommissionRate   DECIMAL(5,2)     NOT NULL DEFAULT 0,   -- platform commission percentage
    IsApproved       BIT              NOT NULL DEFAULT 0,
    ApprovedByUserId UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    ApprovedAt       DATETIME2        NULL,
    IsActive         BIT              NOT NULL DEFAULT 1,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Sellers PRIMARY KEY (Id),
    CONSTRAINT UX_Sellers_UserId UNIQUE (UserId),
    CONSTRAINT UX_Sellers_Slug   UNIQUE (Slug)
);

-- Deferred FK: Products.SellerId now that Sellers exists
ALTER TABLE Products ADD CONSTRAINT FK_Products_Sellers
    FOREIGN KEY (SellerId) REFERENCES Sellers(Id) ON DELETE SET NULL;
ALTER TABLE DiscountUsageLog ADD CONSTRAINT FK_DiscountUsageLog_Customers
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 14  ADDRESSES & DIGITAL WALLET
-- ═══════════════════════════════════════════════════════════════════════════

-- CustomerAddresses: Customer shipping and billing addresses.
CREATE TABLE CustomerAddresses (
    Id                   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CustomerId           UNIQUEIDENTIFIER NOT NULL REFERENCES Customers(Id) ON DELETE CASCADE,
    UserId               UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    AddressType          NVARCHAR(20)     NOT NULL DEFAULT 'Shipping'
                         CONSTRAINT CK_Addresses_Type CHECK(AddressType IN('Shipping','Billing','Both')),
    Label                NVARCHAR(50)     NULL,        -- user-defined nickname e.g. "Home", "Office"
    FullName             NVARCHAR(120)    NOT NULL,
    CompanyName          NVARCHAR(200)    NULL,
    PhoneNumber          NVARCHAR(20)     NOT NULL,
    AlternatePhone       NVARCHAR(20)     NULL,
    AddressLine1         NVARCHAR(500)    NOT NULL,
    AddressLine2         NVARCHAR(500)    NULL,
    City                 NVARCHAR(100)    NOT NULL,
    State                NVARCHAR(100)    NULL,
    PostalCode           NVARCHAR(20)     NULL,
    Country              NCHAR(2)         NOT NULL DEFAULT 'BD',
    IsDefault            BIT              NOT NULL DEFAULT 0,
    DeliveryInstructions NVARCHAR(500)    NULL,
    CreatedAt            DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt            DATETIME2        NULL,
    UpdatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted            BIT              NOT NULL DEFAULT 0,
    RowVersion           ROWVERSION       NULL,
    CONSTRAINT PK_Addresses PRIMARY KEY (Id)
);
CREATE INDEX IX_Addresses_Customer ON CustomerAddresses(CustomerId) WHERE IsDeleted=0;

-- CustomerWallets: Digital wallet per customer.  Balance is authoritative here.
CREATE TABLE CustomerWallets (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CustomerId   UNIQUEIDENTIFIER NOT NULL REFERENCES Customers(Id) ON DELETE CASCADE,
    CurrencyCode NCHAR(3)         NOT NULL DEFAULT 'BDT' REFERENCES Currencies(CurrencyCode),
    Balance      DECIMAL(18,2)    NOT NULL DEFAULT 0 CHECK(Balance >= 0),
    IsActive     BIT              NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_Wallets PRIMARY KEY (Id),
    CONSTRAINT UX_Wallets_Customer UNIQUE (CustomerId)
);

-- WalletTransactions: Complete credit / debit ledger for each wallet.
-- BalanceAfter is an accepted denorm — it is a point-in-time snapshot stored
-- to enable fast display of transaction history without running SUM each time.
CREATE TABLE WalletTransactions (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    WalletId        UNIQUEIDENTIFIER NOT NULL REFERENCES CustomerWallets(Id) ON DELETE CASCADE,
    Amount          DECIMAL(18,2)    NOT NULL,   -- positive = credit; negative = debit
    BalanceAfter    DECIMAL(18,2)    NOT NULL,   -- wallet balance immediately after this transaction
    TransactionType NVARCHAR(30)     NOT NULL
                    CONSTRAINT CK_WalletTx_Type CHECK(TransactionType IN('Credit','Debit','Refund','Cashback','Adjustment','Bonus')),
    Status          NVARCHAR(20)     NOT NULL DEFAULT 'Completed'
                    CONSTRAINT CK_WalletTx_Status CHECK(Status IN('Pending','Completed','Failed','Reversed')),
    Reference       NVARCHAR(100)    NULL,   -- e.g. order number or payment gateway reference
    Description     NVARCHAR(MAX)    NULL,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_WalletTransactions PRIMARY KEY (Id)
);
CREATE INDEX IX_WalletTransactions ON WalletTransactions(WalletId, CreatedAt DESC);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 15  CART
-- ═══════════════════════════════════════════════════════════════════════════

-- Carts: Active shopping baskets.  Guest carts use SessionId only.
-- TaxAmount and ShippingAmount are intentionally absent — they are
-- recalculated fresh at checkout and must not be stale.
CREATE TABLE Carts (
    Id                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CustomerId        UNIQUEIDENTIFIER NULL REFERENCES Customers(Id) ON DELETE CASCADE,
    UserId            UNIQUEIDENTIFIER    NULL REFERENCES Users    (Id),
    SessionId         NVARCHAR(120)    NULL,   -- anonymous guest session token
    SubTotal          DECIMAL(18,2)    NOT NULL DEFAULT 0,
    DiscountAmount    DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Total             DECIMAL(18,2)    NOT NULL DEFAULT 0,
    AppliedDiscountId UNIQUEIDENTIFIER NULL REFERENCES Discounts(Id),
    CouponCode        NVARCHAR(60)     NULL,   -- snapshot of the applied code at add-time
    ExpiresAt         DATETIME2        NULL,   -- cart auto-expires for guest sessions
    CreatedAt         DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt         DATETIME2        NULL,
    UpdatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted         BIT              NOT NULL DEFAULT 0,
    RowVersion        ROWVERSION       NULL,
    CONSTRAINT PK_Carts PRIMARY KEY (Id),
    CONSTRAINT CK_Carts_Owner CHECK(UserId IS NOT NULL OR SessionId IS NOT NULL OR CustomerId IS NOT NULL)
);

-- CartItems: Line items in a cart.  UnitPrice is snapshotted at add-time.
CREATE TABLE CartItems (
    Id         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CartId     UNIQUEIDENTIFIER NOT NULL REFERENCES Carts          (Id) ON DELETE CASCADE,
    ProductId  UNIQUEIDENTIFIER NOT NULL REFERENCES Products        (Id),
    VariantId  UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants (Id),
    BatchId    UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches  (Id),
    Quantity   DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    UnitPrice  DECIMAL(18,2)    NOT NULL,   -- price at the moment the item was added to cart
    TotalPrice DECIMAL(18,2)    NOT NULL,
    AddedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt  DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt  DATETIME2        NULL,
    UpdatedBy  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted  BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION       NULL,
    CONSTRAINT PK_CartItems PRIMARY KEY (Id)
);
CREATE INDEX IX_CartItems_Cart    ON CartItems(CartId)    WHERE IsDeleted=0;
CREATE INDEX IX_CartItems_Product ON CartItems(ProductId) WHERE IsDeleted=0;
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 16  QUOTES (B2B QUOTATION FLOW)
-- ═══════════════════════════════════════════════════════════════════════════

-- Quotes: PosTransactions quotations issued before converting to a formal order.
CREATE TABLE Quotes (
    Id                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    QuoteNo            NVARCHAR(50)     NOT NULL,   -- human-readable reference e.g. QUO-2025-0042
    CustomerId         UNIQUEIDENTIFIER NULL REFERENCES Customers (Id),
    WarehouseId        UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses(Id),
    QuoteDate          DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    ValidUntilDate     DATETIME2        NULL,   -- quote expires and cannot be accepted after this
    SubTotal           DECIMAL(18,2)    NOT NULL DEFAULT 0,
    DiscountAmount     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TaxAmount          DECIMAL(18,2)    NOT NULL DEFAULT 0,
    GrandTotal         DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Status             NVARCHAR(20)     NOT NULL DEFAULT 'Draft'
                       CONSTRAINT CK_Quotes_Status CHECK(Status IN('Draft','Sent','Accepted','Rejected','Converted','Expired')),
    OrderId UNIQUEIDENTIFIER NULL,   -- FK set after Orders
    Notes              NVARCHAR(MAX)    NULL,
    CreatedAt          DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy          UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt          DATETIME2        NULL,
    UpdatedBy          UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted          BIT              NOT NULL DEFAULT 0,
    RowVersion         ROWVERSION       NULL,
    CONSTRAINT PK_Quotes PRIMARY KEY (Id),
    CONSTRAINT UX_Quotes_No UNIQUE (QuoteNo)
);

-- QuoteItems: Line items within a quotation.
CREATE TABLE QuoteItems (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    QuoteId         UNIQUEIDENTIFIER NOT NULL REFERENCES Quotes    (Id) ON DELETE CASCADE,
    ProductId       UNIQUEIDENTIFIER NOT NULL REFERENCES Products   (Id) ON DELETE CASCADE,
    VariantId       UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants(Id),
    Quantity        DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    UnitPrice       DECIMAL(18,2)    NOT NULL,
    DiscountPercent DECIMAL(5,2)     NOT NULL DEFAULT 0,
    DiscountAmount  DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TaxAmount       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    LineTotal       DECIMAL(18,2)    NOT NULL,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_QuoteItems PRIMARY KEY (Id)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 17  ORDERS
-- ───────────────────────────────────────────────────────────────────────────
-- 3NF: PaymentMethod, PaymentStatus, PaymentDate and PaymentTransactionId
--      were columns on Orders in the original schema.  They depend on the
--      Payments table (the payment record), not on the order itself —
--      a transitive dependency.  All four have been moved to Payments.
-- 3NF: InternalNote was a separate column alongside AdminNote.  Both store
--      staff commentary about the same order; merged into AdminNote.
-- ═══════════════════════════════════════════════════════════════════════════

-- Orders: E-commerce customer purchase orders.
CREATE TABLE Orders (
    Id                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderNumber        NVARCHAR(50)     NOT NULL,
    CustomerId         UNIQUEIDENTIFIER NOT NULL REFERENCES Customers(Id),
    UserId             UNIQUEIDENTIFIER    NULL     REFERENCES Users    (Id),
    WarehouseId        UNIQUEIDENTIFIER NULL     REFERENCES Warehouses(Id),   -- fulfilment site
    ShippingAddressId  UNIQUEIDENTIFIER NOT NULL REFERENCES CustomerAddresses(Id),
    BillingAddressId   UNIQUEIDENTIFIER NULL     REFERENCES CustomerAddresses(Id),
    AppliedDiscountId  UNIQUEIDENTIFIER NULL     REFERENCES Discounts(Id),
    StatusCode         NVARCHAR(30)     NOT NULL DEFAULT 'Pending' REFERENCES OrderStatuses(StatusCode),
    OrderDate          DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    OrderConfirmedDate DATETIME2        NULL,
    ShippedDate        DATETIME2        NULL,
    DeliveredDate      DATETIME2        NULL,
    CancellationDate   DATETIME2        NULL,
    CancellationReason NVARCHAR(500)    NULL,
    SubTotal           DECIMAL(18,2)    NOT NULL,
    ShippingAmount     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TaxAmount          DECIMAL(18,2)    NOT NULL DEFAULT 0,
    DiscountAmount     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalAmount        DECIMAL(18,2)    NOT NULL,
    PaidAmount         DECIMAL(18,2)    NOT NULL DEFAULT 0,
    RefundedAmount     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    CustomerNote       NVARCHAR(1000)   NULL,   -- note submitted by customer at checkout
    AdminNote          NVARCHAR(2000)   NULL,   -- internal staff commentary on this order
    CreatedAt          DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy          UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt          DATETIME2        NULL,
    UpdatedBy          UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted          BIT              NOT NULL DEFAULT 0,
    RowVersion         ROWVERSION       NULL,
    CONSTRAINT PK_Orders PRIMARY KEY (Id),
    CONSTRAINT UX_Orders_Number UNIQUE (OrderNumber)
);
CREATE INDEX IX_Orders_Customer ON Orders(CustomerId, OrderDate DESC) WHERE IsDeleted=0;
CREATE INDEX IX_Orders_Status   ON Orders(StatusCode)                 WHERE IsDeleted=0;

-- Deferred FKs that reference Orders
ALTER TABLE Quotes ADD CONSTRAINT FK_Quotes_Orders
    FOREIGN KEY (OrderId) REFERENCES Orders(Id);
ALTER TABLE DiscountUsageLog ADD CONSTRAINT FK_DiscountUsageLog_Orders
    FOREIGN KEY (OrderId) REFERENCES Orders(Id);
ALTER TABLE LoyaltyTransactions ADD CONSTRAINT FK_LoyaltyTransactions_Orders
    FOREIGN KEY (OrderId) REFERENCES Orders(Id);
GO

-- OrderItems: Snapshotted line items.  ProductName and VariantName are stored
-- so receipts remain accurate even after future product renames.
CREATE TABLE OrderItems (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderId         UNIQUEIDENTIFIER NOT NULL REFERENCES Orders         (Id) ON DELETE CASCADE,
    ProductId       UNIQUEIDENTIFIER NOT NULL REFERENCES Products        (Id),
    VariantId       UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants (Id),
    SellerId        UNIQUEIDENTIFIER NULL     REFERENCES Sellers         (Id),
    BatchId         UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches  (Id),
    ProductName     NVARCHAR(200)    NOT NULL,   -- snapshot at order time
    VariantName     NVARCHAR(120)    NULL,        -- snapshot at order time
    SKU             NVARCHAR(50)     NULL,
    Quantity        DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    UnitPrice       DECIMAL(18,2)    NOT NULL,
    UnitCost        DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- for margin reporting
    DiscountPercent DECIMAL(5,2)     NOT NULL DEFAULT 0,
    DiscountAmount  DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TaxAmount       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalPrice      DECIMAL(18,2)    NOT NULL,
    TotalCost       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Notes           NVARCHAR(500)    NULL,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_OrderItems PRIMARY KEY (Id)
);
CREATE INDEX IX_OrderItems_Order   ON OrderItems(OrderId)   WHERE IsDeleted=0;
CREATE INDEX IX_OrderItems_Product ON OrderItems(ProductId) WHERE IsDeleted=0;

-- OrderItemTaxes: Per-line tax breakdown (VAT, surcharges).
CREATE TABLE OrderItemTaxes (
    Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderItemId UNIQUEIDENTIFIER NOT NULL REFERENCES OrderItems(Id) ON DELETE CASCADE,
    TaxRateId   UNIQUEIDENTIFIER NOT NULL REFERENCES TaxRates  (Id),
    TaxName     NVARCHAR(100)    NOT NULL,   -- snapshot of tax name at order time
    TaxRate     DECIMAL(9,4)     NOT NULL,   -- snapshot of rate at order time
    TaxAmount   DECIMAL(18,2)    NOT NULL,
    CreatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted   BIT              NOT NULL DEFAULT 0,
    RowVersion  ROWVERSION       NULL,
    CONSTRAINT PK_OrderItemTaxes PRIMARY KEY (Id)
);

-- OrderBundleSelections: DynamicBundle option picks per order line.
CREATE TABLE OrderBundleSelections (
    Id                      UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderItemId             UNIQUEIDENTIFIER NOT NULL REFERENCES OrderItems        (Id) ON DELETE CASCADE,
    GroupId                 UNIQUEIDENTIFIER NOT NULL REFERENCES BundleOptionGroups(Id),
    VariantId               UNIQUEIDENTIFIER NOT NULL REFERENCES ProductVariants   (Id),
    Quantity                INT              NOT NULL DEFAULT 1,
    PriceAdjustment DECIMAL(18,2)    NOT NULL,   -- snapshot of PriceAdjustment at order time
    CreatedAt               DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy               UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted               BIT              NOT NULL DEFAULT 0,
    RowVersion              ROWVERSION       NULL,
    CONSTRAINT PK_OrderBundleSelections PRIMARY KEY (Id)
);

-- Invoices: Formal invoice document linked to an order.
CREATE TABLE Invoices (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderId        UNIQUEIDENTIFIER NOT NULL REFERENCES Orders(Id) ON DELETE CASCADE,
    InvoiceNumber  NVARCHAR(50)     NOT NULL,
    InvoiceDate    DATETIME2        NOT NULL,
    PaymentDueDate DATETIME2        NULL,   -- for net-terms / credit sales
    SubTotal       DECIMAL(18,2)    NOT NULL,
    TaxAmount      DECIMAL(18,2)    NOT NULL,
    ShippingAmount DECIMAL(18,2)    NOT NULL,
    DiscountAmount DECIMAL(18,2)    NOT NULL,
    TotalAmount    DECIMAL(18,2)    NOT NULL,
    AmountDue      DECIMAL(18,2)    NOT NULL,   -- TotalAmount minus any advance paid
    Notes          NVARCHAR(MAX)    NULL,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt      DATETIME2        NULL,
    UpdatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_Invoices PRIMARY KEY (Id),
    CONSTRAINT UX_Invoices_Number UNIQUE (InvoiceNumber)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 18  PAYMENTS
-- ═══════════════════════════════════════════════════════════════════════════

-- PaymentGateways: Gateway provider configurations.
CREATE TABLE PaymentGateways (
    Id                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    MethodCode        NVARCHAR(40)     NOT NULL REFERENCES PaymentMethods(MethodCode),
    Name              NVARCHAR(100)    NOT NULL,
    Provider          NVARCHAR(80)     NOT NULL,   -- gateway company name e.g. "bKash Ltd"
    LogoUrl           NVARCHAR(MAX)    NULL,
    ConfigurationJson NVARCHAR(MAX)    NULL,        -- encrypted gateway credentials (use Always Encrypted)
    IsActive          BIT              NOT NULL DEFAULT 1,
    IsLiveMode        BIT              NOT NULL DEFAULT 0,   -- 0 = sandbox / test mode
    CreatedAt         DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt         DATETIME2        NULL,
    UpdatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted         BIT              NOT NULL DEFAULT 0,
    RowVersion        ROWVERSION       NULL,
    CONSTRAINT PK_PaymentGateways PRIMARY KEY (Id)
);

-- Payments: Every payment attempt for an order.  Multiple rows = split payment.
-- 3NF: MethodCode, StatusCode, PaidAt all depend on this payment record's PK,
--      not on Orders.  This corrects the 3NF violation where these were on Orders.
CREATE TABLE Payments (
    Id                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderId           UNIQUEIDENTIFIER NOT NULL REFERENCES Orders        (Id) ON DELETE CASCADE,
    MethodCode        NVARCHAR(40)     NOT NULL REFERENCES PaymentMethods(MethodCode),
    Provider          NVARCHAR(60)     NULL,        -- gateway name for this specific transaction
    StatusCode        NVARCHAR(30)     NOT NULL DEFAULT 'Pending' REFERENCES PaymentStatuses(StatusCode),
    Amount            DECIMAL(18,2)    NOT NULL,
    TransactionAmount DECIMAL(18,2)    NOT NULL,    -- may differ from Amount due to partial captures
    GatewayFee        DECIMAL(10,2)    NULL,         -- deducted by gateway on settlement
    RefundedAmount    DECIMAL(18,2)    NOT NULL DEFAULT 0,
    CurrencyCode      NCHAR(3)         NOT NULL DEFAULT 'BDT' REFERENCES Currencies(CurrencyCode),
    TransactionId     NVARCHAR(250)    NULL,         -- gateway-issued transaction reference
    ReferenceNumber   NVARCHAR(50)     NULL,         -- internal reference e.g. bKash transaction ID
    GatewayResponse   NVARCHAR(MAX)    NULL,         -- raw JSON response from gateway (for debugging)
    FailureReason     NVARCHAR(500)    NULL,
    PaidAt            DATETIME2        NULL,          -- timestamp of successful payment confirmation
    RefundedAt        DATETIME2        NULL,          -- timestamp of completed refund
    CreatedAt         DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt         DATETIME2        NULL,
    UpdatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted         BIT              NOT NULL DEFAULT 0,
    RowVersion        ROWVERSION       NULL,
    CONSTRAINT PK_Payments PRIMARY KEY (Id)
);
CREATE UNIQUE INDEX UX_Payments_TransactionId ON Payments(TransactionId) WHERE TransactionId IS NOT NULL;
CREATE INDEX IX_Payments_Order ON Payments(OrderId) WHERE IsDeleted=0;
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 19  SHIPPING & DELIVERY
-- ═══════════════════════════════════════════════════════════════════════════

-- ShippingMethods: Named delivery service configurations.
CREATE TABLE ShippingMethods (
    Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name                  NVARCHAR(100)    NOT NULL,
    Description           NVARCHAR(MAX)    NULL,
    CarrierName           NVARCHAR(100)    NULL,
    BaseCost              DECIMAL(18,2)    NOT NULL DEFAULT 0,
    CostPerKg             DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- per-kg surcharge
    EstimatedDaysMin      INT              NOT NULL DEFAULT 1,
    EstimatedDaysMax      INT              NOT NULL DEFAULT 7,
    IsActive              BIT              NOT NULL DEFAULT 1,
    IsFreeShipping        BIT              NOT NULL DEFAULT 0,
    FreeShippingThreshold DECIMAL(18,2)    NULL,   -- order total above which shipping is free
    DisplayOrder          INT              NOT NULL DEFAULT 0,
    CreatedAt             DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt             DATETIME2        NULL,
    UpdatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted             BIT              NOT NULL DEFAULT 0,
    RowVersion            ROWVERSION       NULL,
    CONSTRAINT PK_ShippingMethods PRIMARY KEY (Id)
);

-- ShippingCarriers: Third-party logistics providers.
CREATE TABLE ShippingCarriers (
    Id                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name              NVARCHAR(100)    NOT NULL,
    LogoUrl           NVARCHAR(MAX)    NULL,
    TrackingUrlPrefix NVARCHAR(MAX)    NULL,   -- append tracking number to form full tracking URL
    IsActive          BIT              NOT NULL DEFAULT 1,
    BaseCost          DECIMAL(18,2)    NOT NULL DEFAULT 0,
    CreatedAt         DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt         DATETIME2        NULL,
    UpdatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted         BIT              NOT NULL DEFAULT 0,
    RowVersion        ROWVERSION       NULL,
    CONSTRAINT PK_ShippingCarriers PRIMARY KEY (Id)
);

-- DeliveryZones: Geographic delivery areas with cost rules.
CREATE TABLE DeliveryZones (
    Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name                  NVARCHAR(120)    NOT NULL,
    Description           NVARCHAR(MAX)    NULL,
    IsActive              BIT              NOT NULL DEFAULT 1,
    BaseDeliveryCost      DECIMAL(18,2)    NOT NULL DEFAULT 0,
    FreeDeliveryThreshold DECIMAL(18,2)    NULL,
    MinDeliveryDays       INT              NULL,
    MaxDeliveryDays       INT              NULL,
    CreatedAt             DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt             DATETIME2        NULL,
    UpdatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted             BIT              NOT NULL DEFAULT 0,
    RowVersion            ROWVERSION       NULL,
    CONSTRAINT PK_DeliveryZones PRIMARY KEY (Id)
);

-- DeliveryZoneRegions: Geographic rows within a delivery zone.
CREATE TABLE DeliveryZoneRegions (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    DeliveryZoneId UNIQUEIDENTIFIER NOT NULL REFERENCES DeliveryZones(Id) ON DELETE CASCADE,
    Country        NCHAR(2)         NOT NULL DEFAULT 'BD',
    State          NVARCHAR(100)    NULL,
    City           NVARCHAR(100)    NULL,
    Area           NVARCHAR(100)    NULL,   -- sub-district or thana
    PostalCode     NVARCHAR(20)     NULL,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt      DATETIME2        NULL,
    UpdatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_DeliveryZoneRegions PRIMARY KEY (Id)
);

-- PickupPoints: Physical customer collection locations.
CREATE TABLE PickupPoints (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    WarehouseId  UNIQUEIDENTIFIER NULL REFERENCES Warehouses(Id),   -- NULL if standalone (not a warehouse)
    Name         NVARCHAR(150)    NOT NULL,
    AddressLine1 NVARCHAR(200)    NOT NULL,
    City         NVARCHAR(100)    NOT NULL,
    PostalCode   NVARCHAR(20)     NULL,
    Phone        NVARCHAR(30)     NOT NULL,
    Latitude     DECIMAL(10,7)    NULL,
    Longitude    DECIMAL(10,7)    NULL,
    OpeningTime  TIME(0)          NULL,
    ClosingTime  TIME(0)          NULL,
    IsActive     BIT              NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_PickupPoints PRIMARY KEY (Id)
);

-- Shipments: Outbound delivery records.  One order can have multiple shipments.
CREATE TABLE Shipments (
    Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderId               UNIQUEIDENTIFIER NOT NULL REFERENCES Orders          (Id) ON DELETE CASCADE,
    ShippingMethodId      UNIQUEIDENTIFIER NULL     REFERENCES ShippingMethods (Id),
    CarrierId             UNIQUEIDENTIFIER NULL     REFERENCES ShippingCarriers(Id),
    WarehouseId           UNIQUEIDENTIFIER NULL     REFERENCES Warehouses      (Id),
    TrackingNumber        NVARCHAR(120)    NOT NULL,
    TrackingUrl           NVARCHAR(500)    NULL,
    StatusCode            NVARCHAR(30)     NOT NULL DEFAULT 'Pending' REFERENCES ShipmentStatuses(StatusCode),
    ShippedDate           DATETIME2        NULL,
    EstimatedDeliveryDate DATETIME2        NULL,
    DeliveredDate         DATETIME2        NULL,
    ShippingCost          DECIMAL(18,2)    NOT NULL DEFAULT 0,
    WeightKg              DECIMAL(8,3)     NOT NULL DEFAULT 0,
    DeliveryNotes         NVARCHAR(500)    NULL,
    CreatedAt             DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt             DATETIME2        NULL,
    UpdatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted             BIT              NOT NULL DEFAULT 0,
    RowVersion            ROWVERSION       NULL,
    CONSTRAINT PK_Shipments PRIMARY KEY (Id)
);
CREATE INDEX IX_Shipments_Order ON Shipments(OrderId) WHERE IsDeleted=0;
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 20  INVENTORY MANAGEMENT
-- ═══════════════════════════════════════════════════════════════════════════

-- StockItems: Per-warehouse per-variant inventory balance.
-- This is the single source of truth for stock levels.
-- AverageCostPrice enables weighted-average COGS calculation.
-- ReservedQuantity prevents overselling committed stock.
CREATE TABLE StockItems (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId        UNIQUEIDENTIFIER NOT NULL REFERENCES Products       (Id),
    VariantId        UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants(Id),
    BatchId          UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches (Id),
    WarehouseId      UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses     (Id),
    QuantityOnHand   DECIMAL(18,2)    NOT NULL DEFAULT 0 CHECK(QuantityOnHand >= 0),
    ReservedQuantity DECIMAL(18,2)    NOT NULL DEFAULT 0 CHECK(ReservedQuantity >= 0),  -- held for open orders
    AverageCostPrice DECIMAL(18,4)    NOT NULL DEFAULT 0,   -- weighted-average cost (used for COGS)
    ReorderLevel     DECIMAL(18,2)    NULL,   -- triggers low-stock alert if QuantityOnHand drops to this
    LastCountDate    DATETIME2        NULL,   -- date of most recent physical stock count
    CountedByUserId  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    LastUpdatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Stock PRIMARY KEY (Id),
    CONSTRAINT UX_Stock UNIQUE (ProductId, VariantId, BatchId, WarehouseId),
    CONSTRAINT CK_StockItems_Reserved CHECK(ReservedQuantity <= QuantityOnHand)
);
CREATE INDEX IX_Stock_Product   ON StockItems(ProductId, WarehouseId) WHERE IsDeleted=0;
CREATE INDEX IX_Stock_Warehouse ON StockItems(WarehouseId)            WHERE IsDeleted=0;

-- StockMovements: Immutable audit ledger of every stock quantity change.
-- QuantityIn and QuantityOut are separate non-negative columns (clearer than a
-- single signed Quantity).  BalanceAfter snapshots the running balance for
-- fast history display without recalculating from the full ledger.
CREATE TABLE StockMovements (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId        UNIQUEIDENTIFIER NOT NULL REFERENCES Products        (Id),
    VariantId        UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants (Id),
    BatchId          UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches  (Id),
    StockItemId      UNIQUEIDENTIFIER NULL     REFERENCES StockItems      (Id),
    FromWarehouseId  UNIQUEIDENTIFIER NULL     REFERENCES Warehouses      (Id),
    ToWarehouseId    UNIQUEIDENTIFIER NULL     REFERENCES Warehouses      (Id),
    MovementTypeCode NVARCHAR(30)     NOT NULL REFERENCES StockMovementTypes(TypeCode),
    QuantityIn       DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- units added to stock
    QuantityOut      DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- units removed from stock
    BalanceAfter     DECIMAL(18,2)    NOT NULL,             -- warehouse balance after this movement
    UnitCost         DECIMAL(18,2)    NULL,                  -- cost per unit for COGS tracking
    ReferenceType    NVARCHAR(40)     NULL,   -- e.g. 'Order','PurchaseOrder','Adjustment','Transfer'
    ReferenceId      UNIQUEIDENTIFIER NULL,   -- GUID of the referencing record
    ReferenceNumber  NVARCHAR(50)     NULL,   -- human-readable reference e.g. order number
    Notes            NVARCHAR(500)    NULL,
    OccurredAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_StockMovements PRIMARY KEY (Id),
    CONSTRAINT CK_StockMovements CHECK(QuantityIn >= 0 AND QuantityOut >= 0)
);
CREATE INDEX IX_StockMovements ON StockMovements(ProductId, OccurredAt DESC) WHERE IsDeleted=0;

-- InventoryAdjustments: Header record for a batch stock correction.
CREATE TABLE InventoryAdjustments (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    AdjustmentNo     NVARCHAR(50)     NOT NULL,
    WarehouseId      UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses(Id),
    AdjustmentDate   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    AdjustmentType   NVARCHAR(30)     NOT NULL
                     CONSTRAINT CK_InvAdj_Type CHECK(AdjustmentType IN('StockCount','DamageWriteOff','ExpiryWriteOff','Correction','Found','Transfer')),
    Reason           NVARCHAR(200)    NOT NULL,
    Notes            NVARCHAR(500)    NULL,
    ApprovedByUserId UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    ApprovedAt       DATETIME2        NULL,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Adjustments PRIMARY KEY (Id),
    CONSTRAINT UX_InventoryAdjustments UNIQUE (AdjustmentNo)
);

-- InventoryAdjustmentLines: Line items for a stock adjustment batch.
-- SystemQuantity vs CountedQuantity enables explicit discrepancy tracking.
CREATE TABLE InventoryAdjustmentLines (
    Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    InventoryAdjustmentId UNIQUEIDENTIFIER NOT NULL REFERENCES InventoryAdjustments(Id) ON DELETE CASCADE,
    ProductId             UNIQUEIDENTIFIER NOT NULL REFERENCES Products             (Id),
    VariantId             UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants      (Id),
    BatchId               UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches       (Id),
    SystemQuantity        DECIMAL(18,2)    NOT NULL,   -- what the system believes the stock level is
    CountedQuantity       DECIMAL(18,2)    NOT NULL,   -- what was physically found during count
    AdjustmentQuantity    DECIMAL(18,2)    NOT NULL,   -- CountedQuantity - SystemQuantity (can be negative)
    UnitCost              DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Remarks               NVARCHAR(255)    NULL,
    CreatedAt             DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted             BIT              NOT NULL DEFAULT 0,
    RowVersion            ROWVERSION       NULL,
    CONSTRAINT PK_InventoryAdjustmentLines PRIMARY KEY (Id)
);

-- StockTransfers + Details: Inter-warehouse transfer tracking.
CREATE TABLE StockTransfers (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TransferNo      NVARCHAR(50)     NOT NULL,
    FromWarehouseId UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses(Id),
    ToWarehouseId   UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses(Id),
    TransferDate    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    Status          NVARCHAR(20)     NOT NULL DEFAULT 'Pending'
                    CONSTRAINT CK_StockTransfers_Status CHECK(Status IN('Pending','InTransit','Completed','Cancelled')),
    Notes           NVARCHAR(500)    NULL,
    CreatedByUserId UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_Transfers PRIMARY KEY (Id),
    CONSTRAINT UX_Transfers_No UNIQUE (TransferNo)
);
CREATE TABLE StockTransferLines (
    Id         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TransferId UNIQUEIDENTIFIER NOT NULL REFERENCES StockTransfers (Id) ON DELETE CASCADE,
    ProductId  UNIQUEIDENTIFIER NOT NULL REFERENCES Products        (Id),
    VariantId  UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants (Id),
    BatchId    UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches  (Id),
    Quantity   DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    CreatedAt  DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted  BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION       NULL,
    CONSTRAINT PK_StockTransferLines PRIMARY KEY (Id)
);

-- ReorderRules: Automatic reorder triggers per product / variant / warehouse.
CREATE TABLE ReorderRules (
    Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId           UNIQUEIDENTIFIER NOT NULL REFERENCES Products       (Id),
    VariantId           UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants(Id),
    WarehouseId         UNIQUEIDENTIFIER NULL     REFERENCES Warehouses     (Id),
    PreferredSupplierId UNIQUEIDENTIFIER NULL     REFERENCES Suppliers      (Id) ON DELETE SET NULL,
    ReorderLevel        DECIMAL(18,2)    NOT NULL,   -- trigger threshold
    ReorderQuantity     DECIMAL(18,2)    NOT NULL,   -- quantity to order when triggered
    NotifyUserId        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),   -- user to alert when triggered
    IsActive            BIT              NOT NULL DEFAULT 1,
    CreatedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt           DATETIME2        NULL,
    UpdatedBy           UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    RowVersion          ROWVERSION       NULL,
    CONSTRAINT PK_ReorderRules PRIMARY KEY (Id)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 21  PROCUREMENT
-- ═══════════════════════════════════════════════════════════════════════════

-- PurchaseOrders: Procurement orders to suppliers.
CREATE TABLE PurchaseOrders (
    Id                   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderNumber          NVARCHAR(50)     NOT NULL,
    SupplierId           UNIQUEIDENTIFIER NOT NULL REFERENCES Suppliers (Id),
    WarehouseId          UNIQUEIDENTIFIER NULL     REFERENCES Warehouses(Id),
    InvoiceNo            NVARCHAR(100)    NULL,   -- supplier's invoice reference number
    CreatedByUserId      UNIQUEIDENTIFIER    NOT NULL REFERENCES Users(Id),
    ApprovedByUserId     UNIQUEIDENTIFIER    NULL     REFERENCES Users(Id),
    ApprovedAt           DATETIME2        NULL,
    Status               NVARCHAR(25)     NOT NULL DEFAULT 'Draft'
                         CONSTRAINT CK_PO_Status CHECK(Status IN('Draft','PendingApproval','Approved','Sent','PartialReceived','Received','Cancelled')),
    OrderDate            DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    ExpectedDeliveryDate DATETIME2        NULL,
    ActualDeliveryDate   DATETIME2        NULL,
    SubTotal             DECIMAL(18,2)    NOT NULL DEFAULT 0,
    DiscountAmount       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalTaxAmount       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TransportCost        DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- freight / logistics charge
    OtherCost            DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- miscellaneous landing charges
    RoundOffAmount       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    GrandTotal           DECIMAL(18,2)    NOT NULL DEFAULT 0,
    PaidAmount           DECIMAL(18,2)    NOT NULL DEFAULT 0,
    DueAmount            DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- GrandTotal - PaidAmount
    TotalItemQuantity    DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Notes                NVARCHAR(MAX)    NULL,
    ShippingAddress      NVARCHAR(MAX)    NULL,
    BillingAddress       NVARCHAR(MAX)    NULL,
    CreatedAt            DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt            DATETIME2        NULL,
    UpdatedBy            UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted            BIT              NOT NULL DEFAULT 0,
    RowVersion           ROWVERSION       NULL,
    CONSTRAINT PK_PurchaseOrders PRIMARY KEY (Id),
    CONSTRAINT UX_PurchaseOrders_No UNIQUE (OrderNumber)
);

-- PurchaseOrderLines: Line items on a purchase order.
CREATE TABLE PurchaseOrderLines (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PurchaseOrderId  UNIQUEIDENTIFIER NOT NULL REFERENCES PurchaseOrders(Id) ON DELETE CASCADE,
    ProductId        UNIQUEIDENTIFIER NOT NULL REFERENCES Products       (Id),
    VariantId        UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants(Id),
    BatchId          UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches (Id),
    ProductName      NVARCHAR(200)    NOT NULL,   -- snapshot for receipt accuracy
    SKU              NVARCHAR(50)     NULL,
    Quantity         DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    ReceivedQuantity DECIMAL(18,2)    NOT NULL DEFAULT 0,
    UnitPrice        DECIMAL(18,2)    NOT NULL,
    DiscountPercent  DECIMAL(5,2)     NOT NULL DEFAULT 0,
    DiscountAmount   DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TaxAmount        DECIMAL(18,2)    NOT NULL DEFAULT 0,
    LineTotal        DECIMAL(18,2)    NOT NULL,
    Notes            NVARCHAR(MAX)    NULL,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_PurchaseLines PRIMARY KEY (Id)
);

-- PurchaseOrderLineTaxes: Per-line tax breakdown for purchase orders.
CREATE TABLE PurchaseOrderLineTaxes (
    Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PurchaseOrderLineId UNIQUEIDENTIFIER NOT NULL REFERENCES PurchaseOrderLines(Id) ON DELETE CASCADE,
    TaxRateId           UNIQUEIDENTIFIER NOT NULL REFERENCES TaxRates           (Id),
    TaxName             NVARCHAR(100)    NOT NULL,
    TaxRate             DECIMAL(9,4)     NOT NULL,
    TaxAmount           DECIMAL(18,2)    NOT NULL,
    CreatedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    RowVersion          ROWVERSION       NULL,
    CONSTRAINT PK_PurchaseOrderLineTaxes PRIMARY KEY (Id)
);

-- PurchaseReturns + Details: Return of delivered goods back to supplier.
CREATE TABLE PurchaseReturns (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PurchaseOrderId UNIQUEIDENTIFIER NOT NULL REFERENCES PurchaseOrders(Id),
    ReturnNo        NVARCHAR(50)     NOT NULL,
    ReturnDate      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    SupplierId      UNIQUEIDENTIFIER NOT NULL REFERENCES Suppliers (Id),
    WarehouseId     UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses(Id),
    SubTotal        DECIMAL(18,2)    NOT NULL DEFAULT 0,
    DiscountAmount  DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TaxAmount       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    GrandTotal      DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Notes           NVARCHAR(500)    NULL,
    CreatedByUserId UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_PurchaseReturns PRIMARY KEY (Id),
    CONSTRAINT UX_PurchReturns_No UNIQUE (ReturnNo)
);
CREATE TABLE PurchaseReturnLines (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PurchaseReturnId UNIQUEIDENTIFIER NOT NULL REFERENCES PurchaseReturns (Id) ON DELETE CASCADE,
    ProductId        UNIQUEIDENTIFIER NOT NULL REFERENCES Products         (Id),
    VariantId        UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants  (Id),
    BatchId          UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches   (Id),
    Quantity         DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    UnitPrice        DECIMAL(18,2)    NOT NULL,
    DiscountAmount   DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TaxAmount        DECIMAL(18,2)    NOT NULL DEFAULT 0,
    LineTotal        DECIMAL(18,2)    NOT NULL,
    Reason           NVARCHAR(500)    NULL,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_PurchaseReturnLines PRIMARY KEY (Id)
);

-- GoodsReceipts + Items: Delivery confirmation records.
CREATE TABLE GoodsReceipts (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PurchaseOrderId  UNIQUEIDENTIFIER NOT NULL REFERENCES PurchaseOrders(Id),
    WarehouseId      UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses    (Id),
    ReceivedByUserId UNIQUEIDENTIFIER    NOT NULL REFERENCES Users         (Id),
    ReceiptNumber    NVARCHAR(50)     NOT NULL,
    ReceiptDate      DATETIME2        NOT NULL,
    Condition        NVARCHAR(50)     NULL,   -- e.g. "Good", "Partial Damage"
    Notes            NVARCHAR(MAX)    NULL,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Receipts PRIMARY KEY (Id),
    CONSTRAINT UX_Receipts_No UNIQUE (ReceiptNumber)
);
CREATE TABLE GoodsReceiptLines (
    Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    GoodsReceiptId      UNIQUEIDENTIFIER NOT NULL REFERENCES GoodsReceipts      (Id) ON DELETE CASCADE,
    PurchaseOrderLineId UNIQUEIDENTIFIER NOT NULL REFERENCES PurchaseOrderLines  (Id),
    ProductId           UNIQUEIDENTIFIER NOT NULL REFERENCES Products             (Id),
    VariantId           UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants      (Id),
    BatchId             UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches       (Id),
    Quantity            DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    UnitCost            DECIMAL(18,2)    NOT NULL,
    Notes               NVARCHAR(MAX)    NULL,
    CreatedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    RowVersion          ROWVERSION       NULL,
    CONSTRAINT PK_GoodsReceiptLines PRIMARY KEY (Id)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 22  RETURNS, REFUNDS & REVIEWS
-- ═══════════════════════════════════════════════════════════════════════════

-- Returns: Customer return request (e-commerce channel).
CREATE TABLE Returns (
    Id                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderId           UNIQUEIDENTIFIER NOT NULL REFERENCES Orders    (Id),
    ReturnNumber      NVARCHAR(50)     NOT NULL,
    RmaNumber         NVARCHAR(50)     NULL,   -- Return Merchandise Authorisation number issued to customer
    ProcessedByUserId UNIQUEIDENTIFIER    NULL REFERENCES Users    (Id),
    Reason            NVARCHAR(500)    NOT NULL,
    Notes             NVARCHAR(MAX)    NULL,
    RequestDate       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    ApprovalDate      DATETIME2        NULL,
    ReceivedDate      DATETIME2        NULL,   -- date goods physically arrived back at warehouse
    RefundDate        DATETIME2        NULL,
    StatusCode        NVARCHAR(30)     NOT NULL DEFAULT 'Requested' REFERENCES ReturnStatuses(StatusCode),
    RefundAmount      DECIMAL(18,2)    NOT NULL DEFAULT 0,
    RefundMethodCode  NVARCHAR(40)     NULL REFERENCES PaymentMethods(MethodCode),
    CreatedAt         DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt         DATETIME2        NULL,
    UpdatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted         BIT              NOT NULL DEFAULT 0,
    RowVersion        ROWVERSION       NULL,
    CONSTRAINT PK_Returns PRIMARY KEY (Id),
    CONSTRAINT UX_Returns_Number UNIQUE (ReturnNumber)
);

-- OrderReturnItems: Specific order lines being returned.
CREATE TABLE OrderReturnItems (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ReturnId     UNIQUEIDENTIFIER NOT NULL REFERENCES Returns    (Id) ON DELETE CASCADE,
    OrderItemId  UNIQUEIDENTIFIER NOT NULL REFERENCES OrderItems (Id),
    Quantity     DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    RefundAmount DECIMAL(18,2)    NOT NULL,
    Reason       NVARCHAR(MAX)    NULL,
    Condition    NVARCHAR(50)     NOT NULL DEFAULT 'Unknown'
                 CONSTRAINT CK_ReturnItems_Cond CHECK(Condition IN('Sealed','Good','Damaged','Faulty','Used','Unknown')),
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_OrderReturnItems PRIMARY KEY (Id)
);

-- PosTransactionReturns: POS-channel in-store return header.
CREATE TABLE PosTransactionReturns (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ReturnNo        NVARCHAR(50)     NOT NULL,
    ReturnDate      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    WarehouseId     UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses(Id),
    CustomerId      UNIQUEIDENTIFIER NULL     REFERENCES Customers (Id),
    TotalAmount     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Notes           NVARCHAR(500)    NULL,
    SaleId          UNIQUEIDENTIFIER NULL,   -- FK set after PosTransactions
    CreatedByUserId UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_PosTransactionReturns PRIMARY KEY (Id),
    CONSTRAINT UX_PosReturns_No UNIQUE (ReturnNo)
);
CREATE TABLE PosTransactionReturnLines (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PosTransactionReturnId UNIQUEIDENTIFIER NOT NULL REFERENCES PosTransactionReturns   (Id) ON DELETE CASCADE,
    ProductId    UNIQUEIDENTIFIER NOT NULL REFERENCES Products        (Id),
    VariantId    UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants (Id),
    BatchId      UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches  (Id),
    Quantity     DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    UnitPrice    DECIMAL(18,2)    NOT NULL,
    LineTotal    DECIMAL(18,2)    NOT NULL,
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_PosTransactionReturnLines PRIMARY KEY (Id)
);

-- RefundRequests: Customer-initiated refund claims for e-commerce orders.
CREATE TABLE RefundRequests (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    OrderId        UNIQUEIDENTIFIER NOT NULL REFERENCES Orders   (Id) ON DELETE CASCADE,
    CustomerId     UNIQUEIDENTIFIER NOT NULL REFERENCES Customers(Id) ON DELETE CASCADE,
    ReturnId       UNIQUEIDENTIFIER NULL     REFERENCES Returns  (Id),
    RefundAmount   DECIMAL(18,2)    NOT NULL,
    Reason         NVARCHAR(MAX)    NOT NULL,
    StatusCode     NVARCHAR(30)     NOT NULL DEFAULT 'Requested' REFERENCES ReturnStatuses(StatusCode),
    AdminNote      NVARCHAR(MAX)    NULL,
    ReturnToWallet BIT              NOT NULL DEFAULT 0,   -- 1 = credit wallet instead of original payment method
    RefundedAt     DATETIME2        NULL,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt      DATETIME2        NULL,
    UpdatedBy      UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_Refunds PRIMARY KEY (Id)
);

-- Reviews: Customer product ratings and text reviews.
-- 1NF: HelpfulCount and NotHelpfulCount are accepted denorms — they are
-- incremented by a trigger and avoid a full COUNT(*) on every review render.
CREATE TABLE Reviews (
    Id                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ProductId          UNIQUEIDENTIFIER NOT NULL REFERENCES Products (Id),
    CustomerId         UNIQUEIDENTIFIER NOT NULL REFERENCES Customers(Id),
    OrderId            UNIQUEIDENTIFIER NULL     REFERENCES Orders   (Id),
    Rating             TINYINT          NOT NULL CHECK(Rating BETWEEN 1 AND 5),
    Title              NVARCHAR(200)    NULL,
    Comment            NVARCHAR(MAX)    NULL,
    MediaUrlsJson      NVARCHAR(MAX)    NULL,   -- JSON array of photo / video URLs attached to review
    IsVerifiedPurchase BIT              NOT NULL DEFAULT 0,
    IsApproved         BIT              NOT NULL DEFAULT 0,
    IsFeatured         BIT              NOT NULL DEFAULT 0,
    HelpfulCount       INT              NOT NULL DEFAULT 0,    -- denorm; updated by trigger
    NotHelpfulCount    INT              NOT NULL DEFAULT 0,
    AdminResponse      NVARCHAR(MAX)    NULL,
    AdminResponseDate  DATETIME2        NULL,
    CreatedAt          DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy          UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt          DATETIME2        NULL,
    UpdatedBy          UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted          BIT              NOT NULL DEFAULT 0,
    RowVersion         ROWVERSION       NULL,
    CONSTRAINT PK_Reviews PRIMARY KEY (Id),
    CONSTRAINT UX_Reviews_CustomerProduct UNIQUE (CustomerId, ProductId)
);
CREATE INDEX IX_Reviews_Product ON Reviews(ProductId) WHERE IsDeleted=0 AND IsApproved=1;

-- ReviewHelpfulness: One vote per user per review.
-- 2NF: IsHelpful depends on the full composite (ReviewId, UserId) key.
CREATE TABLE ReviewHelpfulness (
    ReviewId  UNIQUEIDENTIFIER NOT NULL REFERENCES Reviews(Id) ON DELETE CASCADE,
    UserId    UNIQUEIDENTIFIER    NOT NULL REFERENCES Users  (Id),
    IsHelpful BIT              NOT NULL,
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_ReviewHelpfulness PRIMARY KEY (ReviewId, UserId)
);

-- Wishlists + WishlistItems: Customer saved product lists.
CREATE TABLE Wishlists (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CustomerId       UNIQUEIDENTIFIER NULL REFERENCES Customers(Id) ON DELETE CASCADE,
    UserId           UNIQUEIDENTIFIER    NULL REFERENCES Users    (Id),
    WishlistTypeCode NVARCHAR(20)     NOT NULL DEFAULT 'Personal' REFERENCES WishlistTypes(TypeCode),
    Name             NVARCHAR(100)    NOT NULL DEFAULT 'My Wishlist',
    SharingToken     NVARCHAR(100)    NULL,   -- unique token for public share URL
    IsPublic         BIT              NOT NULL DEFAULT 0,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Wishlists PRIMARY KEY (Id)
);
CREATE TABLE WishlistItems (
    Id         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    WishlistId UNIQUEIDENTIFIER NOT NULL REFERENCES Wishlists      (Id) ON DELETE CASCADE,
    ProductId  UNIQUEIDENTIFIER NOT NULL REFERENCES Products        (Id),
    VariantId  UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants (Id),
    Notes      NVARCHAR(500)    NULL,
    Priority   INT              NOT NULL DEFAULT 0,
    AddedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt  DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt  DATETIME2        NULL,
    UpdatedBy  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted  BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION       NULL,
    CONSTRAINT PK_WishlistItems PRIMARY KEY (Id),
    CONSTRAINT UX_WishlistItems UNIQUE (WishlistId, ProductId, VariantId)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 23  POS SYSTEM
-- ═══════════════════════════════════════════════════════════════════════════

-- Employees: Store staff profiles.  PhotoUrl replaces the inline blob.
CREATE TABLE Employees (
    Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    WarehouseId           UNIQUEIDENTIFIER NULL REFERENCES Warehouses(Id),
    UserId                UNIQUEIDENTIFIER    NULL REFERENCES Users     (Id),
    EmployeeCode          NVARCHAR(50)     NOT NULL,
    FirstName             NVARCHAR(100)    NOT NULL,
    LastName              NVARCHAR(100)    NULL,
    Gender                NVARCHAR(20)     NULL,
    DateOfBirth           DATETIME2        NULL,
    Phone                 NVARCHAR(30)     NULL,
    Email                 NVARCHAR(150)    NULL,
    AddressLine1          NVARCHAR(200)    NULL,
    City                  NVARCHAR(100)    NULL,
    JoiningDate           DATETIME2        NULL,
    TerminationDate       DATETIME2        NULL,
    Designation           NVARCHAR(100)    NULL,
    Department            NVARCHAR(100)    NULL,
    EmployeeType          NVARCHAR(50)     NULL,   -- e.g. Full-time, Part-time, Contract
    Salary                DECIMAL(18,2)    NULL,
    BankName              NVARCHAR(100)    NULL,
    BankAccountNumber     NVARCHAR(50)     NULL,
    NationalId            NVARCHAR(50)     NULL,
    EmergencyContactName  NVARCHAR(150)    NULL,
    EmergencyContactPhone NVARCHAR(30)     NULL,
    PhotoUrl              NVARCHAR(500)    NULL,   -- CDN URL; avoids blob storage on this table
    ShiftPattern          NVARCHAR(50)     NULL,
    IsActive              BIT              NOT NULL DEFAULT 1,
    CreatedAt             DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt             DATETIME2        NULL,
    UpdatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted             BIT              NOT NULL DEFAULT 0,
    RowVersion            ROWVERSION       NULL,
    CONSTRAINT PK_Employees PRIMARY KEY (Id),
    CONSTRAINT UX_Employees_Code UNIQUE (EmployeeCode)
);

-- PosCounters: Named counter positions within a store.
CREATE TABLE PosCounters (
    Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    WarehouseId UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses(Id),
    CounterCode NVARCHAR(50)     NOT NULL,   -- e.g. "COUNTER-01"
    CounterName NVARCHAR(100)    NOT NULL,
    IsActive    BIT              NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt   DATETIME2        NULL,
    UpdatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted   BIT              NOT NULL DEFAULT 0,
    RowVersion  ROWVERSION       NULL,
    CONSTRAINT PK_PosCounters PRIMARY KEY (Id),
    CONSTRAINT UX_PosCounters UNIQUE (WarehouseId, CounterCode)
);

-- PosTerminals: Hardware terminal at a POS counter.
CREATE TABLE PosTerminals (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PosCounterId UNIQUEIDENTIFIER NOT NULL REFERENCES PosCounters(Id),
    TerminalCode NVARCHAR(50)     NOT NULL,
    TerminalName NVARCHAR(100)    NOT NULL,
    MachineName  NVARCHAR(100)    NULL,   -- Windows machine name for identification
    IPAddress    NVARCHAR(50)     NULL,
    PrinterName  NVARCHAR(100)    NULL,   -- receipt printer driver name
    IsActive     BIT              NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_PosTerminals PRIMARY KEY (Id),
    CONSTRAINT UX_PosTerminals UNIQUE (PosCounterId, TerminalCode)
);

-- CashShifts: Cashier shift with opening / closing float reconciliation.
-- TotalSalesAmount and TotalTransactions are accepted denorms — they are
-- updated by the application after each sale and avoid an expensive SUM
-- at shift close.
CREATE TABLE CashShifts (
    Id                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    WarehouseId         UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses  (Id),
    PosCounterId        UNIQUEIDENTIFIER NOT NULL REFERENCES PosCounters (Id),
    PosTerminalId       UNIQUEIDENTIFIER NULL     REFERENCES PosTerminals(Id),
    OpenedByEmployeeId  UNIQUEIDENTIFIER NULL     REFERENCES Employees   (Id),
    ClosedByEmployeeId  UNIQUEIDENTIFIER NULL     REFERENCES Employees   (Id),
    OpenedByUserId      UNIQUEIDENTIFIER    NULL     REFERENCES Users       (Id),
    ClosedByUserId      UNIQUEIDENTIFIER    NULL     REFERENCES Users       (Id),
    OpeningDateTime     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    ClosingDateTime     DATETIME2        NULL,
    OpeningCash         DECIMAL(18,2)    NOT NULL DEFAULT 0,
    ClosingCash         DECIMAL(18,2)    NULL,
    ExpectedCash        DECIMAL(18,2)    NULL,   -- calculated from opening cash + net movements
    CashVariance        DECIMAL(18,2)    NULL,   -- ClosingCash - ExpectedCash
    TotalSalesAmount    DECIMAL(18,2)    NOT NULL DEFAULT 0,   -- running total updated per sale
    TotalTransactions   INT              NOT NULL DEFAULT 0,   -- running count updated per sale
    Status              NVARCHAR(20)     NOT NULL DEFAULT 'Open'
                        CONSTRAINT CK_CashShifts_Status CHECK(Status IN('Open','Closed','Reconciled')),
    Notes               NVARCHAR(500)    NULL,
    CreatedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt           DATETIME2        NULL,
    UpdatedBy           UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted           BIT              NOT NULL DEFAULT 0,
    RowVersion          ROWVERSION       NULL,
    CONSTRAINT PK_CashShifts PRIMARY KEY (Id)
);

-- PosTransactions: A completed POS sale.  CustomerName and CustomerPhone
-- are intentionally kept for walk-in customers who have no profile.
CREATE TABLE PosTransactions (
    Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ReceiptNumber         NVARCHAR(50)     NOT NULL,
    CashShiftId           UNIQUEIDENTIFIER NOT NULL REFERENCES CashShifts  (Id),
    PosCounterId          UNIQUEIDENTIFIER NOT NULL REFERENCES PosCounters (Id),
    PosTerminalId         UNIQUEIDENTIFIER NULL     REFERENCES PosTerminals(Id),
    CashierId             UNIQUEIDENTIFIER    NOT NULL REFERENCES Users       (Id),
    CashierEmployeeId     UNIQUEIDENTIFIER NULL     REFERENCES Employees   (Id),
    CustomerId            UNIQUEIDENTIFIER NULL     REFERENCES Customers   (Id),
    WarehouseId           UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses  (Id),
    AppliedDiscountId     UNIQUEIDENTIFIER NULL     REFERENCES Discounts   (Id),
    SaleDate              DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    SaleType              NVARCHAR(20)     NOT NULL DEFAULT 'Regular'
                          CONSTRAINT CK_PosTxn_SaleType CHECK(SaleType IN('Regular','Takeaway','Delivery','DineIn')),
    FloorTableId          INT              NULL,   -- table number for dine-in restaurant mode
    SubTotal              DECIMAL(18,2)    NOT NULL,
    DiscountAmount        DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalTaxAmount        DECIMAL(18,2)    NOT NULL DEFAULT 0,
    RoundOffAmount        DECIMAL(18,2)    NOT NULL DEFAULT 0,
    GrandTotal            DECIMAL(18,2)    NOT NULL,
    PaidAmount            DECIMAL(18,2)    NOT NULL,
    ChangeAmount          DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalItemQuantity     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    EarnedLoyaltyPoints   INT              NULL,
    RedeemedLoyaltyPoints INT              NULL,
    CouponCode            NVARCHAR(60)     NULL,
    CouponDiscount        DECIMAL(18,2)    NULL,
    CustomerName          NVARCHAR(150)    NULL,   -- walk-in name when no customer profile exists
    CustomerPhone         NVARCHAR(30)     NULL,
    Status                NVARCHAR(20)     NOT NULL DEFAULT 'Completed'
                          CONSTRAINT CK_PosTxn_Status CHECK(Status IN('Completed','Voided','Held','Refunded')),
    VoidReason            NVARCHAR(250)    NULL,
    VoidedBy              UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    VoidedAt              DATETIME2        NULL,
    Notes                 NVARCHAR(500)    NULL,
    CreatedAt             DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt             DATETIME2        NULL,
    UpdatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted             BIT              NOT NULL DEFAULT 0,
    RowVersion            ROWVERSION       NULL,
    CONSTRAINT PK_Sales PRIMARY KEY (Id),
    CONSTRAINT UX_Sales_Receipt UNIQUE (ReceiptNumber)
);
CREATE INDEX IX_Sales_Cashier   ON PosTransactions(CashierId,  SaleDate DESC) WHERE IsDeleted=0;
CREATE INDEX IX_Sales_Shift     ON PosTransactions(CashShiftId)               WHERE IsDeleted=0;
CREATE INDEX IX_Sales_Customer  ON PosTransactions(CustomerId)                WHERE CustomerId IS NOT NULL AND IsDeleted=0;
CREATE INDEX IX_Sales_Warehouse ON PosTransactions(WarehouseId, SaleDate)     WHERE IsDeleted=0;

-- Deferred FKs that reference PosTransactions
ALTER TABLE PosTransactionReturns ADD CONSTRAINT FK_PosTransactionReturns_Sales
    FOREIGN KEY (SaleId) REFERENCES PosTransactions(Id);
ALTER TABLE LoyaltyTransactions ADD CONSTRAINT FK_LoyaltyTransactions_Sales
    FOREIGN KEY (PosTransId) REFERENCES PosTransactions(Id);
ALTER TABLE DiscountUsageLog ADD CONSTRAINT FK_DiscountUsageLog_Sales
    FOREIGN KEY (PosTransactionId) REFERENCES PosTransactions(Id);
GO

-- PosTransactionLines: Line items on a POS sale.
CREATE TABLE PosTransactionLines (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TransactionId   UNIQUEIDENTIFIER NOT NULL REFERENCES PosTransactions (Id) ON DELETE CASCADE,
    ProductId       UNIQUEIDENTIFIER NOT NULL REFERENCES Products         (Id),
    VariantId       UNIQUEIDENTIFIER NULL     REFERENCES ProductVariants  (Id),
    BatchId         UNIQUEIDENTIFIER NULL     REFERENCES ProductBatches   (Id),
    ProductName     NVARCHAR(200)    NOT NULL,   -- snapshot for receipt
    SKU             NVARCHAR(50)     NULL,
    Barcode         NVARCHAR(60)     NULL,
    Quantity        DECIMAL(18,2)    NOT NULL CHECK(Quantity > 0),
    UnitPrice       DECIMAL(18,2)    NOT NULL,
    DiscountPercent DECIMAL(5,2)     NOT NULL DEFAULT 0,
    DiscountAmount  DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TaxAmount       DECIMAL(18,2)    NOT NULL DEFAULT 0,
    LineTotal       DECIMAL(18,2)    NOT NULL,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_PosTransactionLines PRIMARY KEY (Id)
);

-- PosTransactionLineTaxes: Per-line tax breakdown for POS sales.
CREATE TABLE PosTransactionLineTaxes (
    Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PosTransactionLineId UNIQUEIDENTIFIER NOT NULL REFERENCES PosTransactionLines(Id) ON DELETE CASCADE,
    TaxRateId UNIQUEIDENTIFIER NOT NULL REFERENCES TaxRates           (Id),
    TaxName   NVARCHAR(100)    NOT NULL,
    TaxRate   DECIMAL(9,4)     NOT NULL,
    TaxAmount DECIMAL(18,2)    NOT NULL,
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION      NULL,
    CONSTRAINT PK_PosTransactionLineTaxes PRIMARY KEY (Id)
);

-- PosPaymentTenders: Split-tender payment lines for a POS sale.
CREATE TABLE PosPaymentTenders (
    Id            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TransactionId UNIQUEIDENTIFIER NOT NULL REFERENCES PosTransactions(Id) ON DELETE CASCADE,
    MethodCode    NVARCHAR(40)     NOT NULL REFERENCES PaymentMethods (MethodCode),
    Amount        DECIMAL(18,2)    NOT NULL,
    TransactionNo NVARCHAR(100)    NULL,   -- MFS reference or card authorisation number
    CardLast4     NCHAR(4)         NULL,
    PaymentDate   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt     DATETIME2        NULL,
    UpdatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted     BIT              NOT NULL DEFAULT 0,
    RowVersion    ROWVERSION       NULL,
    CONSTRAINT PK_PosPaymentTenders PRIMARY KEY (Id)
);

-- PosTransactionBundleSelections: DynamicBundle option picks on a POS sale.
CREATE TABLE PosTransactionBundleSelections (
    Id                      UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    PosTransactionLineId               UNIQUEIDENTIFIER NOT NULL REFERENCES PosTransactionLines(Id) ON DELETE CASCADE,
    GroupId                 UNIQUEIDENTIFIER NOT NULL REFERENCES BundleOptionGroups (Id),
    VariantId               UNIQUEIDENTIFIER NOT NULL REFERENCES ProductVariants    (Id),
    Quantity                INT              NOT NULL DEFAULT 1,
    PriceAdjustment DECIMAL(18,2)    NOT NULL,
    CreatedAt               DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy               UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted               BIT              NOT NULL DEFAULT 0,
    RowVersion              ROWVERSION       NULL,
    CONSTRAINT PK_PosTransactionBundleSelections PRIMARY KEY (Id)
);

-- CashDrawerEvents: Every cash in / out movement for shift reconciliation.
CREATE TABLE CashDrawerEvents (
    Id            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CashShiftId   UNIQUEIDENTIFIER NOT NULL REFERENCES CashShifts    (Id),
    PerformedBy   UNIQUEIDENTIFIER    NOT NULL REFERENCES Users         (Id),
    TransactionId UNIQUEIDENTIFIER NULL     REFERENCES PosTransactions(Id),
    EventType     NVARCHAR(25)     NOT NULL
                  CONSTRAINT CK_CashDrawer_Type CHECK(EventType IN('Sale','Refund','PaidIn','PaidOut','OpenFloat','CloseCount')),
    Amount        DECIMAL(18,2)    NOT NULL,
    Notes         NVARCHAR(MAX)    NULL,
    OccurredAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted     BIT              NOT NULL DEFAULT 0,
    RowVersion    ROWVERSION       NULL,
    CONSTRAINT PK_CashDrawerEvents PRIMARY KEY (Id)
);

-- DayEndSummaries: Daily reconciliation report per store.
-- Contains accepted denorms that are expensive to recalculate each page load.
CREATE TABLE DayEndSummaries (
    Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    SummaryDate           DATE             NOT NULL,   -- calendar date, not timestamp
    WarehouseId           UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses(Id) ON DELETE CASCADE,
    CashShiftId           UNIQUEIDENTIFIER NULL     REFERENCES CashShifts(Id),
    TotalSalesCount       INT              NOT NULL DEFAULT 0,
    TotalSalesAmount      DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalCashSales        DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalCardSales        DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalMobileSales      DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalReturnAmount     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalDiscount         DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalTaxCollected     DECIMAL(18,2)    NOT NULL DEFAULT 0,
    OpeningCash           DECIMAL(18,2)    NOT NULL DEFAULT 0,
    CashInHand            DECIMAL(18,2)    NOT NULL DEFAULT 0,
    CashOut               DECIMAL(18,2)    NOT NULL DEFAULT 0,
    ExpectedCash          DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Variance              DECIMAL(18,2)    NOT NULL DEFAULT 0,
    TotalItemsSold        INT              NOT NULL DEFAULT 0,
    TotalTransactions     INT              NOT NULL DEFAULT 0,
    NewCustomers          INT              NOT NULL DEFAULT 0,
    ReturningCustomers    INT              NOT NULL DEFAULT 0,
    LoyaltyPointsIssued   DECIMAL(18,2)    NOT NULL DEFAULT 0,
    LoyaltyPointsRedeemed DECIMAL(18,2)    NOT NULL DEFAULT 0,
    Status                NVARCHAR(20)     NOT NULL DEFAULT 'Open'
                          CONSTRAINT CK_DayEnd_Status CHECK(Status IN('Open','Closed','Reconciled')),
    Notes                 NVARCHAR(MAX)    NULL,
    ClosedAt              DATETIME2        NULL,
    ClosedByUserId        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CreatedAt             DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt             DATETIME2        NULL,
    UpdatedBy             UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted             BIT              NOT NULL DEFAULT 0,
    RowVersion            ROWVERSION       NULL,
    CONSTRAINT PK_DayEndSummaries PRIMARY KEY (Id),
    CONSTRAINT UX_DayEndSummaries UNIQUE (SummaryDate, WarehouseId)
);

-- ExpenseCategories + Expenses: Operational cost tracking.
-- and an ExpenseCategoryId FK on the same row — a transitive dependency.
-- The free-text column has been removed; the FK is the sole reference.
CREATE TABLE ExpenseCategories (
    Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name        NVARCHAR(100)    NOT NULL,
    Description NVARCHAR(255)    NULL,
    IsActive    BIT              NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt   DATETIME2        NULL,
    UpdatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted   BIT              NOT NULL DEFAULT 0,
    RowVersion  ROWVERSION       NULL,
    CONSTRAINT PK_ExpenseCategories PRIMARY KEY (Id)
);
CREATE TABLE Expenses (
    Id                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    WarehouseId       UNIQUEIDENTIFIER NOT NULL REFERENCES Warehouses       (Id),
    ExpenseCategoryId UNIQUEIDENTIFIER NULL     REFERENCES ExpenseCategories(Id),
    ExpenseDate       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    Description       NVARCHAR(255)    NULL,
    Amount            DECIMAL(18,2)    NOT NULL CHECK(Amount > 0),
    MethodCode        NVARCHAR(40)     NULL REFERENCES PaymentMethods(MethodCode),
    ReceiptReference  NVARCHAR(100)    NULL,
    CreatedByUserId   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CreatedAt         DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt         DATETIME2        NULL,
    UpdatedBy         UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted         BIT              NOT NULL DEFAULT 0,
    RowVersion        ROWVERSION       NULL,
    CONSTRAINT PK_Expenses PRIMARY KEY (Id)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 24  BLOG / CMS
-- ═══════════════════════════════════════════════════════════════════════════

-- BlogCategories: Post categories.
CREATE TABLE BlogCategories (
    Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name        NVARCHAR(100)    NOT NULL,
    Slug        NVARCHAR(100)    NOT NULL,
    Description NVARCHAR(MAX)    NULL,
    CreatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt   DATETIME2        NULL,
    UpdatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted   BIT              NOT NULL DEFAULT 0,
    RowVersion  ROWVERSION       NULL,
    CONSTRAINT PK_BlogCategories PRIMARY KEY (Id),
    CONSTRAINT UX_BlogCats_Slug UNIQUE (Slug)
);

-- Blogs: Articles with full SEO support and author tracking.
CREATE TABLE Blogs (
    Id               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CategoryId       UNIQUEIDENTIFIER NOT NULL REFERENCES BlogCategories(Id) ON DELETE CASCADE,
    AuthorId         UNIQUEIDENTIFIER    NULL     REFERENCES Users        (Id),
    Title            NVARCHAR(300)    NOT NULL,
    Slug             NVARCHAR(300)    NOT NULL,
    Content          NVARCHAR(MAX)    NOT NULL,
    ShortDescription NVARCHAR(MAX)    NULL,
    ThumbnailUrl     NVARCHAR(MAX)    NULL,
    BannerUrl        NVARCHAR(MAX)    NULL,
    IsPublished      BIT              NOT NULL DEFAULT 0,
    PublishedAt      DATETIME2        NULL,
    ViewCount        INT              NOT NULL DEFAULT 0,   -- incremented asynchronously
    MetaTitle        NVARCHAR(200)    NULL,
    MetaDescription  NVARCHAR(500)    NULL,
    CreatedAt        DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt        DATETIME2        NULL,
    UpdatedBy        UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted        BIT              NOT NULL DEFAULT 0,
    RowVersion       ROWVERSION       NULL,
    CONSTRAINT PK_Blogs PRIMARY KEY (Id),
    CONSTRAINT UX_Blogs_Slug UNIQUE (Slug)
);

-- BlogTags + BlogPostTags: Keyword labels for blog posts.
CREATE TABLE BlogTags (
    Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name      NVARCHAR(80)     NOT NULL,
    Slug      NVARCHAR(80)     NOT NULL,
    CreatedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION      NULL,
    CONSTRAINT PK_BlogTags PRIMARY KEY (Id),
    CONSTRAINT UX_BlogTags_Slug UNIQUE (Slug)
);
CREATE TABLE BlogPostTags (
    BlogId    UNIQUEIDENTIFIER NOT NULL REFERENCES Blogs   (Id) ON DELETE CASCADE,
    BlogTagId UNIQUEIDENTIFIER NOT NULL REFERENCES BlogTags(Id) ON DELETE CASCADE,
    CONSTRAINT PK_BlogPostTags PRIMARY KEY (BlogId, BlogTagId)
);

-- BlogComments: Reader comments with threaded replies via ParentCommentId.
CREATE TABLE BlogComments (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    BlogId          UNIQUEIDENTIFIER NOT NULL REFERENCES Blogs       (Id) ON DELETE CASCADE,
    UserId          UNIQUEIDENTIFIER    NULL     REFERENCES Users       (Id),
    ParentCommentId UNIQUEIDENTIFIER NULL     REFERENCES BlogComments(Id),  -- NULL = top-level comment
    Name            NVARCHAR(120)    NOT NULL,
    Email           NVARCHAR(256)    NOT NULL,
    Content         NVARCHAR(MAX)    NOT NULL,
    IsApproved      BIT              NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_BlogComments PRIMARY KEY (Id)
);

-- StaticPages: CMS pages such as About Us and Privacy Policy.
CREATE TABLE StaticPages (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Title           NVARCHAR(200)    NOT NULL,
    Slug            NVARCHAR(200)    NOT NULL,
    Content         NVARCHAR(MAX)    NOT NULL,
    IsPublished     BIT              NOT NULL DEFAULT 1,
    MetaTitle       NVARCHAR(200)    NULL,
    MetaDescription NVARCHAR(500)    NULL,
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt       DATETIME2        NULL,
    UpdatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_Pages PRIMARY KEY (Id),
    CONSTRAINT UX_Pages_Slug UNIQUE (Slug)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 25  SYSTEM / ADMIN
-- ═══════════════════════════════════════════════════════════════════════════

-- AppSettings: Key-value application configuration store.
CREATE TABLE AppSettings (
    Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Key]       NVARCHAR(120)    NOT NULL,
    Value       NVARCHAR(MAX)    NOT NULL,
    Description NVARCHAR(MAX)    NULL,
    Category    NVARCHAR(60)     NOT NULL,
    DataType    NVARCHAR(20)     NOT NULL DEFAULT 'String'
                CONSTRAINT CK_AppSettings_Type CHECK(DataType IN('String','Bool','Int','Decimal','JSON','DateTime')),
    IsPublic    BIT              NOT NULL DEFAULT 0,   -- 1 = value may be returned to front-end clients
    IsEncrypted BIT              NOT NULL DEFAULT 0,   -- 1 = value is encrypted at rest
    CreatedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt   DATETIME2        NULL,
    UpdatedBy   UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted   BIT              NOT NULL DEFAULT 0,
    RowVersion  ROWVERSION       NULL,
    CONSTRAINT PK_AppSettings PRIMARY KEY (Id),
    CONSTRAINT UX_AppSettings_Key UNIQUE ([Key])
);

-- EmailTemplates: Transactional email template store.
CREATE TABLE EmailTemplates (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name         NVARCHAR(120)    NOT NULL,
    Subject      NVARCHAR(300)    NOT NULL,
    Body         NVARCHAR(MAX)    NOT NULL,   -- HTML body with {{merge_tag}} placeholders
    TemplateType NVARCHAR(60)     NOT NULL,   -- e.g. OrderConfirmation, PasswordReset
    IsActive     BIT              NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_EmailTemplates PRIMARY KEY (Id),
    CONSTRAINT UX_Templates_Type UNIQUE (TemplateType)
);

-- ContactMessages: Public contact form submissions.
CREATE TABLE ContactMessages (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name         NVARCHAR(120)    NOT NULL,
    Email        NVARCHAR(256)    NOT NULL,
    Subject      NVARCHAR(200)    NOT NULL,
    Message      NVARCHAR(MAX)    NOT NULL,
    IsRead       BIT              NOT NULL DEFAULT 0,
    IsReplied    BIT              NOT NULL DEFAULT 0,
    ReplyMessage NVARCHAR(MAX)    NULL,
    RepliedAt    DATETIME2        NULL,
    RepliedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_ContactMessages PRIMARY KEY (Id)
);

-- NewsletterSubscribers: Email marketing opt-in list.
CREATE TABLE NewsletterSubscribers (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Email          NVARCHAR(256)    NOT NULL,
    CustomerId     UNIQUEIDENTIFIER NULL REFERENCES Customers(Id),
    IsActive       BIT              NOT NULL DEFAULT 1,
    SubscribedAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    UnsubscribedAt DATETIME2        NULL,
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_NewsletterSubscribers PRIMARY KEY (Id),
    CONSTRAINT UX_Subscribers_Email UNIQUE (Email)
);

-- SearchKeywords: Aggregated search keyword analytics for autocomplete.
CREATE TABLE SearchKeywords (
    Id             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Keyword        NVARCHAR(200)    NOT NULL,
    SearchCount    INT              NOT NULL DEFAULT 1,   -- incremented on each search
    LastSearchedAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt      DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted      BIT              NOT NULL DEFAULT 0,
    RowVersion     ROWVERSION       NULL,
    CONSTRAINT PK_SearchKeywords PRIMARY KEY (Id),
    CONSTRAINT UX_Searches_Keyword UNIQUE (Keyword)
);

-- Notifications: In-app, email and SMS notification records.
CREATE TABLE Notifications (
    Id         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    UserId     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id) ON DELETE CASCADE,
    Title      NVARCHAR(200)    NOT NULL,
    Message    NVARCHAR(1000)   NOT NULL,
    Type       NVARCHAR(50)     NOT NULL,   -- e.g. OrderUpdate, Promo, LowStock
    Link       NVARCHAR(500)    NULL,        -- relative URL to navigate on click
    ImageUrl   NVARCHAR(500)    NULL,
    IsRead     BIT              NOT NULL DEFAULT 0,
    ReadAt     DATETIME2        NULL,
    IsSent     BIT              NOT NULL DEFAULT 0,
    SentAt     DATETIME2        NULL,
    TargetRole NVARCHAR(256)    NULL,   -- role ID for broadcast; NULL = individual user
    SendEmail  BIT              NOT NULL DEFAULT 0,
    SendSms    BIT              NOT NULL DEFAULT 0,
    CreatedAt  DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy  UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted  BIT              NOT NULL DEFAULT 0,
    RowVersion ROWVERSION       NULL,
    CONSTRAINT PK_Notifications PRIMARY KEY (Id)
);
CREATE INDEX IX_Notifications_User ON Notifications(UserId) WHERE IsDeleted=0 AND IsRead=0;

-- ActivityLogs: User-visible action history (separate from AuditLogs).
CREATE TABLE ActivityLogs (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    UserId       UNIQUEIDENTIFIER    NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    ActivityType NVARCHAR(80)     NOT NULL,   -- e.g. ProductViewed, OrderPlaced, LoginSuccess
    Description  NVARCHAR(MAX)    NOT NULL,
    EntityType   NVARCHAR(60)     NULL,        -- e.g. "Product", "Order"
    EntityId     UNIQUEIDENTIFIER NULL,        -- GUID of the referenced entity
    IpAddress    NVARCHAR(50)     NULL,
    UserAgent    NVARCHAR(500)    NULL,
    OccurredAt   DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_ActivityLogs PRIMARY KEY (Id)
);
CREATE INDEX IX_ActivityLog_User ON ActivityLogs(UserId, OccurredAt DESC) WHERE IsDeleted=0;

-- AuditLogs: Immutable security and compliance audit trail.
-- 1NF: EntityId is NVARCHAR(60) to accommodate both GUID and integer PKs.
-- 3NF: This table is append-only — no UpdatedAt, UpdatedBy, or IsDeleted.
--      Adding those columns would imply records can be changed, which would
--      violate the immutability requirement of a compliance audit log.
-- Id is BIGINT for high-volume write throughput on clustered index.
CREATE TABLE AuditLogs (
    Id         BIGINT           IDENTITY(1,1) NOT NULL,
    UserId     UNIQUEIDENTIFIER    NULL REFERENCES Users(Id) ON DELETE SET NULL,
    Action     NVARCHAR(120)    NOT NULL,   -- e.g. Create, Update, Delete, Login
    EntityName NVARCHAR(100)    NOT NULL,   -- table / aggregate name e.g. "Products"
    EntityId   NVARCHAR(60)     NULL,        -- GUID or int PK as string
    OldValues  NVARCHAR(MAX)    NULL,        -- JSON snapshot of old state
    NewValues  NVARCHAR(MAX)    NULL,        -- JSON snapshot of new state
    IpAddress  NVARCHAR(50)     NULL,
    UserAgent  NVARCHAR(500)    NULL,
    Details    NVARCHAR(1000)   NULL,
    OccurredAt DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_AuditLogs PRIMARY KEY (Id)
);
CREATE INDEX IX_AuditLog_User       ON AuditLogs(UserId,     OccurredAt DESC);
CREATE INDEX IX_AuditLog_Entity     ON AuditLogs(EntityName, EntityId);
CREATE INDEX IX_AuditLog_Date ON AuditLogs(OccurredAt DESC);

-- SupportTickets: Customer service tickets with priority and assignment.
CREATE TABLE SupportTickets (
    Id           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TicketNumber NVARCHAR(30)     NOT NULL,
    UserId       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    AssignedToId UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    OrderId      UNIQUEIDENTIFIER NULL REFERENCES Orders(Id),
    Subject      NVARCHAR(200)    NOT NULL,
    Description  NVARCHAR(MAX)    NOT NULL,
    Category     NVARCHAR(50)     NULL,   -- e.g. Order, Payment, Technical, General
    Priority     NVARCHAR(20)     NOT NULL DEFAULT 'Normal'
                 CONSTRAINT CK_Tickets_Priority CHECK(Priority IN('Low','Normal','High','Urgent')),
    Status       NVARCHAR(20)     NOT NULL DEFAULT 'Open'
                 CONSTRAINT CK_Tickets_Status CHECK(Status IN('Open','Pending','Resolved','Closed')),
    AdminNote    NVARCHAR(MAX)    NULL,
    ResolvedAt   DATETIME2        NULL,
    ClosedAt     DATETIME2        NULL,
    CreatedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    UpdatedAt    DATETIME2        NULL,
    UpdatedBy    UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted    BIT              NOT NULL DEFAULT 0,
    RowVersion   ROWVERSION       NULL,
    CONSTRAINT PK_Tickets PRIMARY KEY (Id),
    CONSTRAINT UX_Tickets_No UNIQUE (TicketNumber)
);

-- SupportTicketMessages: Threaded conversation on a ticket.
CREATE TABLE SupportTicketMessages (
    Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    SupportTicketId UNIQUEIDENTIFIER NOT NULL REFERENCES SupportTickets(Id) ON DELETE CASCADE,
    SenderId        UNIQUEIDENTIFIER    NULL     REFERENCES Users          (Id),
    Message         NVARCHAR(MAX)    NOT NULL,
    IsFromAdmin     BIT              NOT NULL DEFAULT 0,
    AttachmentUrl   NVARCHAR(500)    NULL,   -- CDN URL of attached file
    CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       UNIQUEIDENTIFIER    NULL REFERENCES Users(Id),
    IsDeleted       BIT              NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION       NULL,
    CONSTRAINT PK_SupportTicketMessages PRIMARY KEY (Id)
);
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SECTION 26  COMPUTED VIEWS
-- ───────────────────────────────────────────────────────────────────────────
-- 1NF requires that derived / computed values not be stored as columns.
-- These four views replace every such column removed during normalisation.
-- Application code should always query these views for the values listed
-- rather than storing the data directly.
-- ═══════════════════════════════════════════════════════════════════════════

-- vw_ProductStats: Replaces Products.RatingAverage and Products.ReviewCount.
-- RatingAverage and ReviewCount are aggregations of the Reviews table.
-- Storing them on Products is a 1NF violation (derived data).
CREATE OR ALTER VIEW vw_ProductStats AS
SELECT
    p.Id                                                               AS ProductId,
    COUNT(r.Id)                                                        AS ReviewCount,
    ISNULL(CAST(AVG(CAST(r.Rating AS FLOAT)) AS DECIMAL(3,2)), 0.00)  AS RatingAverage,
    SUM(CASE WHEN r.IsVerifiedPurchase = 1 THEN 1 ELSE 0 END)         AS VerifiedReviewCount
FROM Products p
LEFT JOIN Reviews r ON r.ProductId = p.Id AND r.IsDeleted = 0 AND r.IsApproved = 1
WHERE p.IsDeleted = 0
GROUP BY p.Id;
GO

-- vw_CustomerStats: Replaces TotalOrders, TotalPurchases, TotalSpent, OrderCount,
-- LastOrderDate which were stored on Customers in both original schemas.
-- All five are aggregations of the Orders table — 1NF violations.
CREATE OR ALTER VIEW vw_CustomerStats AS
SELECT
    c.Id                                                              AS CustomerId,
    COUNT(o.Id)                                                       AS OrderCount,
    ISNULL(SUM(o.TotalAmount), 0)                                    AS TotalSpent,
    MAX(o.OrderDate)                                                  AS LastOrderDate,
    COUNT(CASE WHEN o.StatusCode = 'Delivered' THEN 1 END)           AS CompletedOrders,
    ISNULL(SUM(o.RefundedAmount), 0)                                 AS TotalRefunded
FROM Customers c
LEFT JOIN Orders o ON o.CustomerId = c.Id AND o.IsDeleted = 0
WHERE c.IsDeleted = 0
GROUP BY c.Id;
GO

-- vw_StockAvailability: Real-time available quantity per product / variant / warehouse.
-- AvailableQty = QuantityOnHand - ReservedQuantity.
-- NeedsReorder = 1 when AvailableQty drops to or below the configured reorder threshold.
CREATE OR ALTER VIEW vw_StockAvailability AS
SELECT
    si.ProductId,
    si.VariantId,
    si.BatchId,
    si.WarehouseId,
    w.Name                                                            AS WarehouseName,
    w.SiteType,
    p.Name                                                            AS ProductName,
    pv.Name                                                           AS VariantName,
    si.QuantityOnHand,
    si.ReservedQuantity,
    (si.QuantityOnHand - si.ReservedQuantity)                        AS AvailableQty,
    si.ReorderLevel,
    si.AverageCostPrice,
    CASE WHEN (si.QuantityOnHand - si.ReservedQuantity)
              <= ISNULL(si.ReorderLevel, p.ReorderLevel)
         THEN 1 ELSE 0 END                                            AS NeedsReorder
FROM StockItems si
JOIN  Products         p  ON p.Id  = si.ProductId
JOIN  Warehouses       w  ON w.Id  = si.WarehouseId
LEFT JOIN ProductVariants pv ON pv.Id = si.VariantId
WHERE si.IsDeleted = 0;
GO

-- vw_CustomerLoyaltyBalance: Current loyalty point balance per customer.
-- The balance is derived from the LoyaltyTransactions ledger and must never
-- be stored redundantly.  NextExpiryDate identifies the earliest batch of
-- points scheduled to expire so the front-end can display a reminder.
CREATE OR ALTER VIEW vw_CustomerLoyaltyBalance AS
SELECT
    CustomerId,
    SUM(Points)                                                       AS CurrentBalance,
    SUM(CASE WHEN Points > 0 THEN Points ELSE 0 END)                 AS TotalEarned,
    SUM(CASE WHEN Points < 0 THEN ABS(Points) ELSE 0 END)            AS TotalRedeemed,
    COUNT(CASE WHEN TransactionType = 'Earn' THEN 1 END)             AS EarnTransactions,
    MIN(CASE WHEN IsUsed = 0 AND ExpiryDate IS NOT NULL
             THEN ExpiryDate END)                                      AS NextExpiryDate
FROM LoyaltyTransactions
WHERE IsDeleted = 0
GROUP BY CustomerId;
GO

-- ═══════════════════════════════════════════════════════════════════════════
-- SEED DATA
-- ═══════════════════════════════════════════════════════════════════════════

INSERT INTO OrderStatuses(StatusCode,DisplayName,SortOrder,IsTerminal) VALUES
('Pending','Pending',1,0),('Confirmed','Confirmed',2,0),
('Processing','Processing',3,0),('Shipped','Shipped',4,0),
('Delivered','Delivered',5,1),('Cancelled','Cancelled',6,1),('Refunded','Refunded',7,1);

INSERT INTO PaymentStatuses(StatusCode,DisplayName) VALUES
('Pending','Pending'),('Completed','Completed'),('Failed','Failed'),
('Refunded','Refunded'),('PartialRefund','Partial Refund');

INSERT INTO PaymentMethods(MethodCode,DisplayName,IsOnline,IsActive,SortOrder) VALUES
('Cash','Cash',0,1,1),('Card','Credit/Debit Card',1,1,2),
('bKash','bKash',1,1,3),('Nagad','Nagad',1,1,4),
('Rocket','Rocket/DBBL',1,1,5),('Wallet','Store Wallet',1,1,6),
('COD','Cash on Delivery',0,1,7),('GiftCard','Gift Card',0,1,8),
('BankTransfer','Bank Transfer',1,1,9);

INSERT INTO ShipmentStatuses(StatusCode,DisplayName,SortOrder) VALUES
('Pending','Pending',1),('Packed','Packed',2),('Dispatched','Dispatched',3),
('InTransit','In Transit',4),('Delivered','Delivered',5),('Returned','Returned',6);

INSERT INTO ReturnStatuses(StatusCode,DisplayName,SortOrder) VALUES
('Requested','Requested',1),('Approved','Approved',2),('Rejected','Rejected',3),
('Received','Received',4),('Processing','Processing',5),
('Refunded','Refunded',6),('Completed','Completed',7);

INSERT INTO StockMovementTypes(TypeCode,DisplayName,IsInbound) VALUES
('Purchase','Purchase Receipt',1),('Sale','Sale',0),('Return','Customer Return',1),
('Transfer','Warehouse Transfer',1),('Adjustment','StockItems Adjustment',1),
('Damage','Damage Write-off',0),('WriteOff','Expiry Write-off',0),
('Found','StockItems Found',1),('Opening','Opening StockItems',1);

INSERT INTO DiscountTypes(TypeCode,DisplayName) VALUES
('Percentage','Percentage Off'),('Fixed','Fixed Amount Off'),
('BOGO','Buy One Get One'),('FreeShipping','Free Shipping'),('BuyXGetY','Buy X Get Y');

INSERT INTO CustomerTiers(TierCode,DisplayName,MinLifetimeSpend,DiscountPct,PointsMultiplier,SortOrder) VALUES
('Bronze','Bronze',0,0,1.0,1),('Silver','Silver',10000,2,1.5,2),
('Gold','Gold',50000,5,2.0,3),('Platinum','Platinum',150000,8,3.0,4);

INSERT INTO ProductConditions(ConditionCode,DisplayName) VALUES
('New','Brand New'),('Refurbished','Refurbished'),
('Used','Used / Pre-owned'),('Damaged','Damaged (Discounted)');

INSERT INTO WishlistTypes(TypeCode,DisplayName) VALUES
('Personal','Personal'),('Registry','Gift Registry'),('Public','Public / Shareable');

INSERT INTO Currencies(CurrencyCode,Name,Symbol,ExchangeRate,DecimalPlaces,IsBaseCurrency,IsActive) VALUES
('BDT','Bangladeshi Taka','৳',1.0,0,1,1),
('USD','US Dollar','$',110.5,2,0,1),
('GBP','British Pound','£',140.2,2,0,1);

INSERT INTO TaxRates(TaxCode,TaxName,TaxType,Rate,IsPercentage,IsInclusive,IsDefault,IsActive,Country,ApplyToShipping,Priority,CreatedBy) VALUES
('BD-VAT-15','Standard VAT 15%','Percentage',15.00,1,0,1,1,'BD',0,1,NULL),
('BD-VAT-0', 'Zero Rate',        'Percentage', 0.00,1,0,0,1,'BD',0,2,NULL),
('BD-VAT-5', 'Reduced Rate 5%',  'Percentage', 5.00,1,0,0,1,'BD',0,3,NULL);

INSERT INTO Warehouses(Code,Name,SiteType,City,Country,IsDefault,IsActive,CreatedBy) VALUES
('WH-MAIN','Main Warehouse','Warehouse','Dhaka','BD',1,1,NULL),
('ST-MAIN','Main Store',    'Store',    'Dhaka','BD',0,1,NULL);

INSERT INTO AttributeTypes(Name,Slug,UiType,AffectsPrice,AffectsSku,AffectsImage,AffectsStock,IsFilterable,CreatedBy) VALUES
('Color',  'color',   'Swatch',  0,1,1,1,1,NULL),
('Size',   'size',    'Button',  0,1,0,1,1,NULL),
('Material','material','Dropdown',0,0,0,0,1,NULL),
('RAM',    'ram',     'Button',  1,1,0,1,1,NULL),
('Storage','storage', 'Button',  1,1,0,1,1,NULL);

INSERT INTO AppSettings([Key],Value,Category,DataType,IsPublic,IsEncrypted,CreatedBy) VALUES
('store.name',          'NEXUS Store','Store',  'String',1,0,NULL),
('store.baseCurrency',  'BDT',        'Store',  'String',1,0,NULL),
('store.country',       'BD',         'Store',  'String',1,0,NULL),
('feature.onlineStore', 'true',       'Feature','Bool',  0,0,NULL),
('feature.guestCheckout','true',      'Feature','Bool',  0,0,NULL),
('feature.reviews',     'true',       'Feature','Bool',  0,0,NULL),
('feature.wallet',      'true',       'Feature','Bool',  0,0,NULL),
('feature.pos',         'true',       'Feature','Bool',  0,0,NULL),
('feature.marketplace', 'false',      'Feature','Bool',  0,0,NULL),
('feature.loyalty',     'true',       'Feature','Bool',  0,0,NULL),
('loyalty.pointsPerTaka','1',         'Loyalty','Int',   0,0,NULL),
('loyalty.takaPerPoint', '0.01',      'Loyalty','Decimal',0,0,NULL),
('inventory.lowStockAlert','5',       'Inventory','Int', 0,0,NULL);

/*
═══════════════════════════════════════════════════════════════════════════════
 TABLE & VIEW COUNT SUMMARY
═══════════════════════════════════════════════════════════════════════════════
 Sec  1  Lookup / Enum          :  10
 Sec  2  Identity               :   7  Users, Roles, UserRoles, UserClaims,
                                       RoleClaims, UserLogins, UserTokens
 Sec  3  RBAC & Security        :   5  Permissions, RolePermissions, Menus,
                                       RoleMenus, UserRefreshTokens
 Sec  4  Currencies & Tax       :   2
 Sec  5  Warehouses             :   1
 Sec  6  Catalog Reference      :   8  Brands, Categories, Suppliers, Colors,
                                       Units, Tags, BrandCategories, (+TaxRates)
 Sec  7  Discounts              :   3  Discounts, DiscountApplicability,
                                       DiscountUsageLog
 Sec  8  EAV Attributes         :   2
 Sec  9  Products               :  13  Products, ProductSupplierLinks,
                                       ProductPriceHistories, ProductBatches,
                                       ProductVariants, ProductAttributeLinks,
                                       VariantAttributeOptions, VariantAttributeMatrix,
                                       ProductSpecifications, ProductSpecValues,
                                       ProductImages, ProductTaxRates, ProductTags
 Sec 10  ProductMedia / Blobs          :   4
 Sec 11  Bundles                :   3
 Sec 12  ProductCollections/FlashDeals      :   6  ProductCollections, ProductCollectionItems,
                                       PriceLists, PriceListItems,
                                       FlashDeals, FlashDealProducts
 Sec 13  Customers/Loyalty      :   4  Customers, CustomerProfiles,
                                       LoyaltyTransactions, Sellers
 Sec 14  CustomerAddresses & Wallet     :   3
 Sec 15  Cart                   :   2
 Sec 16  Quotes                 :   2
 Sec 17  Orders                 :   5  Orders, OrderItems, OrderItemTaxes,
                                       OrderBundleSelections, Invoices
 Sec 18  Payments               :   2
 Sec 19  Shipping               :   6
 Sec 20  Inventory              :   7  StockItems, StockMovements,
                                       InventoryAdjustments, InvAdjDetails,
                                       StockTransfers, StockTransferLines,
                                       ReorderRules
 Sec 21  Procurement            :   8  PurchaseOrders, POItems, POItemTaxes,
                                       PurchaseReturns, PurchReturnDetails,
                                       GoodsReceipts, GoodsReceiptLines
 Sec 22  Returns / Reviews      :   9  Returns, OrderReturnItems, PosTransactionReturns,
                                       PosTransactionReturnLines, RefundRequests,
                                       Reviews, ReviewHelpfulness,
                                       Wishlists, WishlistItems
 Sec 23  POS System             :  14  Employees, PosCounters, PosTerminals,
                                       CashShifts, PosTransactions, PosTxnItems,
                                       PosTxnItemTaxes, PosPaymentTenders,
                                       PosTransactionBundleSelections, CashDrawerEvents,
                                       DayEndSummaries, ExpenseCategories, Expenses
 Sec 24  Blog / CMS             :   6
 Sec 25  System / Admin         :  10  AppSettings, EmailTemplates,
                                       ContactMessages, NewsletterSubscribers,
                                       SearchKeywords, Notifications,
                                       ActivityLogs, AuditLogs,
                                       SupportTickets, SupportTicketMessages
 Sec 26  Views                  :   4  vw_ProductStats, vw_CustomerStats,
                                       vw_StockAvailability,
                                       vw_CustomerLoyaltyBalance
───────────────────────────────────────────────────────────────────────────────
 TOTAL TABLES  : 115
 TOTAL VIEWS   :   4
 TOTAL INDEXES :  43+
═══════════════════════════════════════════════════════════════════════════════
*/
