using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class SupplierEndpoints
{
    public static void MapSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suppliers").WithTags("Suppliers");

        group.MapGet("/", async (
            [AsParameters] GetSuppliersRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Suppliers
                .Where(s => !s.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(s => s.Name.Contains(request.Search) || s.SupplierCode.Contains(request.Search) || s.Phone.Contains(request.Search));

            if (!string.IsNullOrWhiteSpace(request.SupplierType))
                query = query.Where(s => s.SupplierType == request.SupplierType);

            if (request.IsActive.HasValue)
                query = query.Where(s => s.IsActive == request.IsActive);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(s => s.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new SupplierListResponse(
                    s.Id, s.SupplierCode, s.Name, s.CompanyName, s.ContactPerson,
                    s.Phone, s.Email, s.SupplierType, s.PaymentTerms, s.LeadTimeDays, s.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetSuppliers")
        .WithSummary("Get paginated suppliers");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var supplier = await context.Suppliers
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (supplier == null)
                return Results.NotFound(new { error = "Supplier not found" });

            var response = new SupplierDetailResponse(
                supplier.Id, supplier.SupplierCode, supplier.Name, supplier.CompanyName,
                supplier.ContactPerson, supplier.Phone, supplier.AlternatePhone, supplier.Email,
                supplier.AddressLine1, supplier.AddressLine2, supplier.City, supplier.State,
                supplier.PostalCode, supplier.Country, supplier.SupplierType,
                supplier.TaxRegistrationNo, supplier.PaymentTerms, supplier.LeadTimeDays,
                supplier.Notes, supplier.IsActive, supplier.CreatedAt);

            return Results.Ok(new { data = response });
        })
        .WithName("GetSupplierById")
        .WithSummary("Get supplier with details");

        group.MapPost("/", async (CreateSupplierRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.Suppliers.AnyAsync(s => s.Phone == request.Phone && !s.IsDeleted, ct);
            if (exists)
                return Results.Conflict(new { error = "Supplier with this phone already exists" });

            var supplier = new Suppliers
            {
                Id = Guid.NewGuid(),
                SupplierCode = $"SUP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
                Name = request.Name,
                CompanyName = request.CompanyName,
                ContactPerson = request.ContactPerson,
                Phone = request.Phone,
                AlternatePhone = request.AlternatePhone,
                Email = request.Email,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City ?? "Dhaka",
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country ?? "Bangladesh",
                SupplierType = request.SupplierType ?? "MANUFACTURER",
                TaxRegistrationNo = request.TaxRegistrationNo,
                PaymentTerms = request.PaymentTerms,
                LeadTimeDays = request.LeadTimeDays,
                Notes = request.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/suppliers/{supplier.Id}", new { data = new { supplier.Id, supplier.SupplierCode, supplier.Name } });
        })
        .WithName("CreateSupplier")
        .WithSummary("Create a new supplier");

        group.MapPut("/{id:guid}", async (Guid id, UpdateSupplierRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var supplier = await context.Suppliers.FindAsync(new object[] { id }, ct);
            if (supplier == null || supplier.IsDeleted)
                return Results.NotFound(new { error = "Supplier not found" });

            supplier.Name = request.Name ?? supplier.Name;
            supplier.CompanyName = request.CompanyName;
            supplier.ContactPerson = request.ContactPerson;
            supplier.Phone = request.Phone ?? supplier.Phone;
            supplier.AlternatePhone = request.AlternatePhone;
            supplier.Email = request.Email;
            supplier.AddressLine1 = request.AddressLine1 ?? supplier.AddressLine1;
            supplier.AddressLine2 = request.AddressLine2;
            supplier.City = request.City ?? supplier.City;
            supplier.State = request.State;
            supplier.PostalCode = request.PostalCode;
            supplier.Country = request.Country ?? supplier.Country;
            supplier.SupplierType = request.SupplierType ?? supplier.SupplierType;
            supplier.TaxRegistrationNo = request.TaxRegistrationNo;
            supplier.PaymentTerms = request.PaymentTerms;
            supplier.LeadTimeDays = request.LeadTimeDays;
            supplier.Notes = request.Notes;
            supplier.IsActive = request.IsActive ?? supplier.IsActive;
            supplier.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { supplier.Id } });
        })
        .WithName("UpdateSupplier")
        .WithSummary("Update supplier");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var supplier = await context.Suppliers.FindAsync(new object[] { id }, ct);
            if (supplier == null || supplier.IsDeleted)
                return Results.NotFound(new { error = "Supplier not found" });

            supplier.IsDeleted = true;
            supplier.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteSupplier")
        .WithSummary("Soft delete supplier");

        group.MapPost("/{id:guid}/toggle-active", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var supplier = await context.Suppliers.FindAsync(new object[] { id }, ct);
            if (supplier == null || supplier.IsDeleted)
                return Results.NotFound(new { error = "Supplier not found" });

            supplier.IsActive = !supplier.IsActive;
            supplier.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new { supplier.Id, supplier.IsActive } });
        })
        .WithName("ToggleSupplierActive")
        .WithSummary("Toggle supplier active status");

        group.MapGet("/types", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var types = await context.Suppliers
                .Select(s => s.SupplierType)
                .Distinct()
                .ToListAsync(ct);

            return Results.Ok(new { data = types });
        })
        .WithName("GetSupplierTypes")
        .WithSummary("Get supplier types");

        group.MapGet("/stats", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var stats = new
            {
                TotalSuppliers = await context.Suppliers.Where(s => !s.IsDeleted).CountAsync(ct),
                ActiveSuppliers = await context.Suppliers.Where(s => !s.IsDeleted && s.IsActive).CountAsync(ct),
            };

            return Results.Ok(new { data = stats });
        })
        .WithName("GetSupplierStats")
        .WithSummary("Get supplier statistics");
    }
}

public record GetSuppliersRequest(
    int PageIndex = 0, int PageSize = 20, string? Search = null,
    string? SupplierType = null, bool? IsActive = null);

public record SupplierListResponse(
    Guid Id, string SupplierCode, string Name, string? CompanyName,
    string? ContactPerson, string Phone, string? Email,
    string? SupplierType, string? PaymentTerms, int? LeadTimeDays, bool IsActive);

public record SupplierDetailResponse(
    Guid Id, string SupplierCode, string Name, string? CompanyName,
    string? ContactPerson, string Phone, string? AlternatePhone, string? Email,
    string? AddressLine1, string? AddressLine2, string? City, string? State,
    string? PostalCode, string Country, string? SupplierType,
    string? TaxRegistrationNo, string? PaymentTerms, int? LeadTimeDays,
    string? Notes, bool IsActive, DateTime CreatedAt);

public record CreateSupplierRequest(
    string Name, string Phone, string? CompanyName, string? ContactPerson,
    string? AlternatePhone, string? Email,
    string? AddressLine1, string? AddressLine2, string? City, string? State,
    string? PostalCode, string? Country, string? SupplierType,
    string? TaxRegistrationNo, string? PaymentTerms, int? LeadTimeDays, string? Notes);

public record UpdateSupplierRequest(
    string? Name, string? Phone, string? CompanyName, string? ContactPerson,
    string? AlternatePhone, string? Email,
    string? AddressLine1, string? AddressLine2, string? City, string? State,
    string? PostalCode, string? Country, string? SupplierType,
    string? TaxRegistrationNo, string? PaymentTerms, int? LeadTimeDays,
    string? Notes, bool? IsActive);