using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<Users> userManager, RoleManager<Roles> roleManager)
    {
        // Skip seeding for non-SQL providers or if database doesn't exist yet
        try
        {
            if (!await context.Database.CanConnectAsync())
            {
                return;
            }
        }
        catch
        {
            return;
        }

        try
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
            await SeedCategories(context);
            await SeedBrands(context);
            await SeedUnits(context);
            await SeedColors(context);
            await SeedTags(context);
            await SeedCustomerTiers(context);
            await SeedPaymentStatuses(context);
            await SeedOrderStatuses(context);
            await SeedShippingMethods(context);
            await SeedWarehouses(context);
            await SeedProducts(context);
            await SeedCustomers(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seeding error: {ex.Message}");
        }
    }

    private static async Task SeedRoles(RoleManager<Roles> roleManager)
    {
        var roles = new[] { "Admin", "Manager", "Cashier", "Customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleEntity = new Roles();
                roleEntity.Name = role;
                roleEntity.IsActive = true;
                roleEntity.Description = $"{role} role";
                await roleManager.CreateAsync(roleEntity);
            }
        }
    }

    private static async Task SeedUsers(UserManager<Users> userManager)
    {
        var adminEmail = "admin@ecommerce.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new Users();
            admin.UserName = adminEmail;
            admin.Email = adminEmail;
            admin.FirstName = "Admin";
            admin.LastName = "User";
            admin.IsActive = true;
            admin.CreatedAt = DateTime.UtcNow;
            admin.PreferredLanguage = "en";
            admin.TimeZone = "UTC";
            
            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }

    private static async Task SeedCategories(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync()) return;

        var categories = new List<Categories>
        {
            new() { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics", Description = "Electronic devices and accessories", IsFeatured = true, IsActive = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Smartphones", Slug = "smartphones", Description = "Latest smartphones", IsFeatured = true, IsActive = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Laptops", Slug = "laptops", Description = "Laptops and computers", IsFeatured = true, IsActive = true, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Audio", Slug = "audio", Description = "Headphones and speakers", IsFeatured = true, IsActive = true, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Wearables", Slug = "wearables", Description = "Smart watches and fitness trackers", IsActive = true, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Tablets", Slug = "tablets", Description = "Tablets and iPads", IsActive = true, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Accessories", Slug = "accessories", Description = "Phone and laptop accessories", IsActive = true, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Gaming", Slug = "gaming", Description = "Gaming consoles and accessories", IsActive = true, DisplayOrder = 8, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Cameras", Slug = "cameras", Description = "Digital cameras and camcorders", IsActive = true, DisplayOrder = 9, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Networking", Slug = "networking", Description = "Routers and networking equipment", IsActive = true, DisplayOrder = 10, CreatedAt = DateTime.UtcNow }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBrands(ApplicationDbContext context)
    {
        if (await context.Brands.AnyAsync()) return;

        var brands = new List<Brands>
        {
            new() { Id = Guid.NewGuid(), BrandCode = "APPLE", Name = "Apple", Slug = "apple", CountryOfOrigin = "US", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "SAMSUNG", Name = "Samsung", Slug = "samsung", CountryOfOrigin = "KR", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "SONY", Name = "Sony", Slug = "sony", CountryOfOrigin = "JP", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "LG", Name = "LG", Slug = "lg", CountryOfOrigin = "KR", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "DELL", Name = "Dell", Slug = "dell", CountryOfOrigin = "US", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "HP", Name = "HP", Slug = "hp", CountryOfOrigin = "US", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "LENOVO", Name = "Lenovo", Slug = "lenovo", CountryOfOrigin = "CN", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "ASUS", Name = "Asus", Slug = "asus", CountryOfOrigin = "TW", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "JBL", Name = "JBL", Slug = "jbl", CountryOfOrigin = "US", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), BrandCode = "BOSE", Name = "Bose", Slug = "bose", CountryOfOrigin = "US", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        context.Brands.AddRange(brands);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUnits(ApplicationDbContext context)
    {
        if (await context.Units.AnyAsync()) return;

        var units = new List<Units>
        {
            new() { Id = Guid.NewGuid(), Name = "Piece", ShortName = "pc", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Kilogram", ShortName = "kg", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Gram", ShortName = "g", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Liter", ShortName = "L", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Meter", ShortName = "m", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Centimeter", ShortName = "cm", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Box", ShortName = "box", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Pack", ShortName = "pack", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        context.Units.AddRange(units);
        await context.SaveChangesAsync();
    }

    private static async Task SeedColors(ApplicationDbContext context)
    {
        if (await context.Colors.AnyAsync()) return;

        var colors = new List<Colors>
        {
            new() { Id = Guid.NewGuid(), Name = "Black", HexCode = "#000000", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "White", HexCode = "#FFFFFF", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Gray", HexCode = "#808080", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Silver", HexCode = "#C0C0C0", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Gold", HexCode = "#FFD700", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Blue", HexCode = "#0000FF", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Red", HexCode = "#FF0000", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Green", HexCode = "#008000", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Pink", HexCode = "#FFC0CB", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Purple", HexCode = "#800080", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        context.Colors.AddRange(colors);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTags(ApplicationDbContext context)
    {
        if (await context.Tags.AnyAsync()) return;

        var tags = new List<Tags>
        {
            new() { Id = Guid.NewGuid(), Name = "New Arrival", Slug = "new-arrival", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Best Seller", Slug = "best-seller", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Featured", Slug = "featured", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Sale", Slug = "sale", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Hot", Slug = "hot", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Trending", Slug = "trending", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Limited Edition", Slug = "limited-edition", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Out of Stock", Slug = "out-of-stock", CreatedAt = DateTime.UtcNow }
        };

        context.Tags.AddRange(tags);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCustomerTiers(ApplicationDbContext context)
    {
        if (await context.CustomerTiers.AnyAsync()) return;

        var tiers = new List<CustomerTiers>
        {
            new() { TierCode = "BRONZE", DisplayName = "Bronze", MinLifetimeSpend = 0, PointsMultiplier = 1.0m, DiscountPct = 0, SortOrder = 1 },
            new() { TierCode = "SILVER", DisplayName = "Silver", MinLifetimeSpend = 10000, PointsMultiplier = 1.5m, DiscountPct = 2, SortOrder = 2 },
            new() { TierCode = "GOLD", DisplayName = "Gold", MinLifetimeSpend = 50000, PointsMultiplier = 2.0m, DiscountPct = 5, SortOrder = 3 },
            new() { TierCode = "PLATINUM", DisplayName = "Platinum", MinLifetimeSpend = 100000, PointsMultiplier = 2.5m, DiscountPct = 10, SortOrder = 4 }
        };

        context.CustomerTiers.AddRange(tiers);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPaymentStatuses(ApplicationDbContext context)
    {
        if (await context.PaymentStatuses.AnyAsync()) return;

        var statuses = new List<PaymentStatuses>
        {
            new() { StatusCode = "PENDING", DisplayName = "Pending" },
            new() { StatusCode = "PAID", DisplayName = "Paid" },
            new() { StatusCode = "FAILED", DisplayName = "Failed" },
            new() { StatusCode = "REFUNDED", DisplayName = "Refunded" },
            new() { StatusCode = "CANCELLED", DisplayName = "Cancelled" }
        };

        context.PaymentStatuses.AddRange(statuses);
        await context.SaveChangesAsync();
    }

    private static async Task SeedOrderStatuses(ApplicationDbContext context)
    {
        if (await context.OrderStatuses.AnyAsync()) return;

        var statuses = new List<OrderStatuses>
        {
            new() { StatusCode = "PENDING", DisplayName = "Pending", SortOrder = 1, IsTerminal = false },
            new() { StatusCode = "CONFIRMED", DisplayName = "Confirmed", SortOrder = 2, IsTerminal = false },
            new() { StatusCode = "PROCESSING", DisplayName = "Processing", SortOrder = 3, IsTerminal = false },
            new() { StatusCode = "SHIPPED", DisplayName = "Shipped", SortOrder = 4, IsTerminal = false },
            new() { StatusCode = "DELIVERED", DisplayName = "Delivered", SortOrder = 5, IsTerminal = true },
            new() { StatusCode = "CANCELLED", DisplayName = "Cancelled", SortOrder = 6, IsTerminal = true },
            new() { StatusCode = "RETURNED", DisplayName = "Returned", SortOrder = 7, IsTerminal = true }
        };

        context.OrderStatuses.AddRange(statuses);
        await context.SaveChangesAsync();
    }

    private static async Task SeedShippingMethods(ApplicationDbContext context)
    {
        if (await context.ShippingMethods.AnyAsync()) return;

        var methods = new List<ShippingMethods>
        {
            new() { Id = Guid.NewGuid(), Name = "Standard Delivery", Description = "Delivery within 5-7 business days", BaseCost = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Express Delivery", Description = "Delivery within 2-3 business days", BaseCost = 150, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Next Day Delivery", Description = "Delivery next business day", BaseCost = 250, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Pickup Point", Description = "Delivery to pickup point", BaseCost = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Free Shipping", Description = "Free delivery for orders above 1000", BaseCost = 0, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        context.ShippingMethods.AddRange(methods);
        await context.SaveChangesAsync();
    }

    private static async Task SeedWarehouses(ApplicationDbContext context)
    {
        if (await context.Warehouses.AnyAsync()) return;

        var warehouses = new List<Warehouses>
        {
            new() { Id = Guid.NewGuid(), Code = "WH001", Name = "Main Warehouse", AddressLine1 = "123 Warehouse St", City = "Dhaka", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Code = "WH002", Name = "Secondary Warehouse", AddressLine1 = "456 Storage Ave", City = "Chittagong", IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        context.Warehouses.AddRange(warehouses);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProducts(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync()) return;

        var category = await context.Categories.FirstAsync(c => c.Name == "Smartphones");
        var brand = await context.Brands.FirstAsync(b => b.Name == "Apple");
        var unit = await context.Units.FirstAsync(u => u.Name == "Piece");

        var products = new List<Products>
        {
            new() { Id = Guid.NewGuid(), ProductCode = "IPHONE15-128", Name = "iPhone 15 128GB", Slug = "iphone-15-128gb", CategoryId = category.Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 70000, SalePrice = 89990, OriginalPrice = 95000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "IPHONE15-256", Name = "iPhone 15 256GB", Slug = "iphone-15-256gb", CategoryId = category.Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 80000, SalePrice = 99990, OriginalPrice = 105000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "IPHONE15PRO-128", Name = "iPhone 15 Pro 128GB", Slug = "iphone-15-pro-128gb", CategoryId = category.Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 100000, SalePrice = 129990, OriginalPrice = 135000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "IPHONE15PRO-256", Name = "iPhone 15 Pro 256GB", Slug = "iphone-15-pro-256gb", CategoryId = category.Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 110000, SalePrice = 139990, OriginalPrice = 145000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "IPHONE15PROMAX-256", Name = "iPhone 15 Pro Max 256GB", Slug = "iphone-15-pro-max-256gb", CategoryId = category.Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 120000, SalePrice = 149990, OriginalPrice = 155000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "AIRPODS-PRO2", Name = "AirPods Pro 2nd Gen", Slug = "airpods-pro-2nd-gen", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Audio")).Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 18000, SalePrice = 24990, OriginalPrice = 27000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "AIRPODS-MAX", Name = "AirPods Max", Slug = "airpods-max", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Audio")).Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 35000, SalePrice = 49990, OriginalPrice = 55000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "APPLE-WATCH-S9", Name = "Apple Watch Series 9", Slug = "apple-watch-series-9", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Wearables")).Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 28000, SalePrice = 39990, OriginalPrice = 45000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "APPLE-WATCH-ULTRA2", Name = "Apple Watch Ultra 2", Slug = "apple-watch-ultra-2", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Wearables")).Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 65000, SalePrice = 84990, OriginalPrice = 90000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "MACBOOK-AIR-M3", Name = "MacBook Air M3", Slug = "macbook-air-m3", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Laptops")).Id, BrandId = brand.Id, UnitId = unit.Id, CostPrice = 95000, SalePrice = 129990, OriginalPrice = 140000, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        var samsungBrand = await context.Brands.FirstAsync(b => b.Name == "Samsung");

        var samsungProducts = new List<Products>
        {
            new() { Id = Guid.NewGuid(), ProductCode = "SAMSUNG-S24", Name = "Samsung Galaxy S24", Slug = "samsung-galaxy-s24", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Smartphones")).Id, BrandId = samsungBrand.Id, UnitId = unit.Id, CostPrice = 65000, SalePrice = 84990, OriginalPrice = 90000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "SAMSUNG-S24PLUS", Name = "Samsung Galaxy S24+", Slug = "samsung-galaxy-s24-plus", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Smartphones")).Id, BrandId = samsungBrand.Id, UnitId = unit.Id, CostPrice = 75000, SalePrice = 99990, OriginalPrice = 105000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "SAMSUNG-S24ULTRA", Name = "Samsung Galaxy S24 Ultra", Slug = "samsung-galaxy-s24-ultra", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Smartphones")).Id, BrandId = samsungBrand.Id, UnitId = unit.Id, CostPrice = 110000, SalePrice = 139990, OriginalPrice = 145000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "SAMSUNG-A55", Name = "Samsung Galaxy A55", Slug = "samsung-galaxy-a55", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Smartphones")).Id, BrandId = samsungBrand.Id, UnitId = unit.Id, CostPrice = 35000, SalePrice = 45990, OriginalPrice = 50000, IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), ProductCode = "SAMSUNG-TAB-S9", Name = "Samsung Galaxy Tab S9", Slug = "samsung-galaxy-tab-s9", CategoryId = (await context.Categories.FirstAsync(c => c.Name == "Tablets")).Id, BrandId = samsungBrand.Id, UnitId = unit.Id, CostPrice = 55000, SalePrice = 74990, OriginalPrice = 80000, IsActive = true, CreatedAt = DateTime.UtcNow }
        };

        context.Products.AddRange(samsungProducts);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCustomers(ApplicationDbContext context)
    {
        if (await context.Customers.AnyAsync()) return;
        
        var customers = new List<Customers>
        {
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS001", CustomerType = "Individual", Email = "john.doe@email.com", Phone = "+8801712345678", City = "Dhaka", Country = "BD", Balance = 0, LoyaltyPoints = 500, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS002", CustomerType = "Individual", Email = "jane.smith@email.com", Phone = "+8801712345679", City = "Chittagong", Country = "BD", Balance = 0, LoyaltyPoints = 1200, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS003", CustomerType = "Corporate", Email = "bob.johnson@email.com", Phone = "+8801712345680", City = "Dhaka", Country = "BD", Balance = 5000, LoyaltyPoints = 5000, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS004", CustomerType = "Individual", Email = "alice.williams@email.com", Phone = "+8801712345681", City = "Sylhet", Country = "BD", Balance = 0, LoyaltyPoints = 200, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS005", CustomerType = "Individual", Email = "charlie.brown@email.com", Phone = "+8801712345682", City = "Dhaka", Country = "BD", Balance = 0, LoyaltyPoints = 15000, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS006", CustomerType = "Corporate", Email = "david.miller@email.com", Phone = "+8801712345683", City = "Chittagong", Country = "BD", Balance = 10000, LoyaltyPoints = 25000, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS007", CustomerType = "Individual", Email = "emma.davis@email.com", Phone = "+8801712345684", City = "Dhaka", Country = "BD", Balance = 0, LoyaltyPoints = 800, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS008", CustomerType = "Individual", Email = "frank.wilson@email.com", Phone = "+8801712345685", City = "Rajshahi", Country = "BD", Balance = 0, LoyaltyPoints = 350, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS009", CustomerType = "Individual", Email = "grace.lee@email.com", Phone = "+8801712345686", City = "Khulna", Country = "BD", Balance = 0, LoyaltyPoints = 2200, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), CustomerCode = "CUS010", CustomerType = "Individual", Email = "henry.taylor@email.com", Phone = "+8801712345687", City = "Dhaka", Country = "BD", Balance = 0, LoyaltyPoints = 150, IsActive = true, RegistrationDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        };

        context.Customers.AddRange(customers);
        await context.SaveChangesAsync();
    }
}
