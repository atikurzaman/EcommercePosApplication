using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", async (
            [AsParameters] GetCustomersRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Customers
                .Include(c => c.CustomerProfiles).ThenInclude(p => p.TierCodeNavigation)
                .Include(c => c.User)
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.Phone.Contains(request.Search) || c.Email.Contains(request.Search) || c.CustomerCode.Contains(request.Search));

            if (request.IsActive.HasValue)
                query = query.Where(c => c.IsActive == request.IsActive);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(c => c.RegistrationDate)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CustomerListResponse(
                    c.Id, c.CustomerCode, c.CustomerType, c.Phone, c.Email,
                    c.CustomerProfiles != null ? c.CustomerProfiles.TierCodeNavigation.DisplayName : "Regular",
                    c.LoyaltyPoints, c.IsActive, c.RegistrationDate, c.LastPurchaseDate))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetCustomers")
        .WithSummary("Get paginated customers");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var customer = await context.Customers
                .Include(c => c.User)
                .Include(c => c.CustomerProfiles).ThenInclude(p => p.TierCodeNavigation)
                .Include(c => c.CustomerAddresses)
                .Include(c => c.Orders).ThenInclude(o => o.StatusCodeNavigation)
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (customer == null)
                return Results.NotFound(new { error = "Customer not found" });

            var response = new CustomerDetailResponse(
                customer.Id, customer.CustomerCode, customer.CustomerType,
                customer.Phone, customer.AlternatePhone, customer.Email,
                customer.DateOfBirth, customer.Gender, customer.CompanyName, customer.TaxNumber,
                customer.AddressLine1, customer.City, customer.Country,
                customer.Balance, customer.CreditLimit, customer.LoyaltyPoints,
                customer.RegistrationDate, customer.LastPurchaseDate, customer.IsActive,
                customer.CustomerProfiles != null ? new CustomerTierInfoResponse(
                    customer.CustomerProfiles.TierCode,
                    customer.CustomerProfiles.TierCodeNavigation.DisplayName,
                    customer.CustomerProfiles.TierCodeNavigation.DiscountPct,
                    customer.CustomerProfiles.TierCodeNavigation.PointsMultiplier) : null,
                customer.CustomerAddresses.Select(a => new CustomerAddressResponse(
                    a.Id, a.AddressType, a.Label, a.FullName, a.PhoneNumber,
                    a.AddressLine1, a.AddressLine2, a.City, a.State, a.PostalCode, a.IsDefault)).ToList(),
                customer.Orders.OrderByDescending(o => o.OrderDate).Take(10).Select(o => new CustomerOrderResponse(
                    o.Id, o.OrderNumber, o.StatusCodeNavigation.DisplayName, o.TotalAmount, o.OrderDate)).ToList());

            return Results.Ok(new { data = response });
        })
        .WithName("GetCustomerById")
        .WithSummary("Get customer with details");

        group.MapPost("/", async (CreateCustomerRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.Customers.AnyAsync(c => c.Phone == request.Phone && !c.IsDeleted, ct);
            if (exists)
                return Results.Conflict(new { error = "Customer with this phone already exists" });

            var customer = new Customers
            {
                Id = Guid.NewGuid(),
                CustomerCode = $"CUST-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
                CustomerType = request.CustomerType ?? "RETAIL",
                Phone = request.Phone,
                AlternatePhone = request.AlternatePhone,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                CompanyName = request.CompanyName,
                TaxNumber = request.TaxNumber,
                AddressLine1 = request.AddressLine1,
                City = request.City ?? "Dhaka",
                Country = request.Country ?? "Bangladesh",
                CreditLimit = request.CreditLimit ?? 0,
                IsActive = true,
                RegistrationDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Customers.Add(customer);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/customers/{customer.Id}", new { data = new { customer.Id, customer.CustomerCode } });
        })
        .WithName("CreateCustomer")
        .WithSummary("Create a new customer");

        group.MapPut("/{id:guid}", async (Guid id, UpdateCustomerRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var customer = await context.Customers.FindAsync(new object[] { id }, ct);
            if (customer == null || customer.IsDeleted)
                return Results.NotFound(new { error = "Customer not found" });

            customer.Phone = request.Phone ?? customer.Phone;
            customer.AlternatePhone = request.AlternatePhone;
            customer.Email = request.Email;
            customer.DateOfBirth = request.DateOfBirth;
            customer.Gender = request.Gender;
            customer.CompanyName = request.CompanyName;
            customer.TaxNumber = request.TaxNumber;
            customer.AddressLine1 = request.AddressLine1 ?? customer.AddressLine1;
            customer.City = request.City ?? customer.City;
            customer.Country = request.Country ?? customer.Country;
            customer.CreditLimit = request.CreditLimit ?? customer.CreditLimit;
            customer.IsActive = request.IsActive;
            customer.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { customer.Id } });
        })
        .WithName("UpdateCustomer")
        .WithSummary("Update customer");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var customer = await context.Customers.FindAsync(new object[] { id }, ct);
            if (customer == null || customer.IsDeleted)
                return Results.NotFound(new { error = "Customer not found" });

            customer.IsDeleted = true;
            customer.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteCustomer")
        .WithSummary("Soft delete customer");

        group.MapPost("/{id:guid}/toggle-active", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var customer = await context.Customers.FindAsync(new object[] { id }, ct);
            if (customer == null || customer.IsDeleted)
                return Results.NotFound(new { error = "Customer not found" });

            customer.IsActive = !customer.IsActive;
            customer.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { customer.Id, customer.IsActive } });
        })
        .WithName("ToggleCustomerActive")
        .WithSummary("Toggle customer active status");

        group.MapGet("/stats", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var today = DateTime.UtcNow.Date;
            var stats = new
            {
                TotalCustomers = await context.Customers.Where(c => !c.IsDeleted).CountAsync(ct),
                ActiveCustomers = await context.Customers.Where(c => !c.IsDeleted && c.IsActive).CountAsync(ct),
                NewCustomersToday = await context.Customers.Where(c => !c.IsDeleted && c.RegistrationDate >= today).CountAsync(ct),
                TotalLoyaltyPoints = await context.Customers.SumAsync(c => c.LoyaltyPoints, ct),
            };

            return Results.Ok(new { data = stats });
        })
        .WithName("GetCustomerStats")
        .WithSummary("Get customer statistics");

        group.MapGet("/addresses/{customerId:guid}", async (Guid customerId, ApplicationDbContext context, CancellationToken ct) =>
        {
            var addresses = await context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && !a.IsDeleted)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.CreatedAt)
                .Select(a => new CustomerAddressResponse(
                    a.Id, a.AddressType, a.Label, a.FullName, a.PhoneNumber,
                    a.AddressLine1, a.AddressLine2, a.City, a.State, a.PostalCode, a.IsDefault))
                .ToListAsync(ct);

            return Results.Ok(new { data = addresses });
        })
        .WithName("GetCustomerAddresses")
        .WithSummary("Get customer addresses");
    }
}

public record GetCustomersRequest(
    int PageIndex = 0, int PageSize = 20, string? Search = null, bool? IsActive = null);

public record CustomerListResponse(
    Guid Id, string CustomerCode, string CustomerType, string Phone, string? Email,
    string? TierName, int LoyaltyPoints, bool IsActive, DateTime RegistrationDate, DateTime? LastPurchaseDate);

public record CustomerDetailResponse(
    Guid Id, string CustomerCode, string CustomerType,
    string Phone, string? AlternatePhone, string? Email,
    DateTime? DateOfBirth, string? Gender, string? CompanyName, string? TaxNumber,
    string? AddressLine1, string? City, string? Country,
    decimal Balance, decimal? CreditLimit, int LoyaltyPoints,
    DateTime RegistrationDate, DateTime? LastPurchaseDate, bool IsActive,
    CustomerTierInfoResponse? Tier,
    List<CustomerAddressResponse> Addresses,
    List<CustomerOrderResponse> RecentOrders);

public record CustomerTierInfoResponse(string TierCode, string DisplayName, decimal DiscountPct, decimal PointsMultiplier);

public record CustomerAddressResponse(
    Guid Id, string AddressType, string? Label, string FullName, string PhoneNumber,
    string AddressLine1, string? AddressLine2, string City, string? State, string? PostalCode, bool IsDefault);

public record CustomerOrderResponse(
    Guid Id, string OrderNumber, string Status, decimal TotalAmount, DateTime OrderDate);

public record CreateCustomerRequest(
    string Phone, string? CustomerType, string? AlternatePhone, string? Email,
    DateTime? DateOfBirth, string? Gender, string? CompanyName, string? TaxNumber,
    string? AddressLine1, string? City, string? Country, decimal? CreditLimit);

public record UpdateCustomerRequest(
    string? Phone, string? AlternatePhone, string? Email,
    DateTime? DateOfBirth, string? Gender, string? CompanyName, string? TaxNumber,
    string? AddressLine1, string? City, string? Country, decimal? CreditLimit, bool IsActive);