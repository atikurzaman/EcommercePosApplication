using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Supplier;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class SupplierEndpoints
{
    public static void MapSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suppliers").WithTags("Suppliers");

        group.MapGet("/", async (
            [AsParameters] GetSuppliers.Query query,
            GetSuppliers.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetSuppliers")
            .WithSummary("Get paginated suppliers");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetSupplierById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetSupplierById.Query(id), ct)).ToHttpResult())
            .WithName("GetSupplierById")
            .WithSummary("Get supplier by id");

        group.MapPost("/", async (
            [FromBody] CreateSupplier.Command command,
            CreateSupplier.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/suppliers"))
            .AddEndpointFilter<ValidationFilter<CreateSupplier.Command>>()
            .WithName("CreateSupplier")
            .WithSummary("Create a new supplier");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateSupplierBody body,
            UpdateSupplier.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateSupplier.Command(
                id, body.Name, body.Phone, body.CompanyName, body.ContactPerson,
                body.AlternatePhone, body.Email, body.AddressLine1, body.AddressLine2,
                body.City, body.State, body.PostalCode, body.Country,
                body.SupplierType, body.TaxRegistrationNo, body.PaymentTerms,
                body.LeadTimeDays, body.Notes, body.IsActive), ct)).ToHttpResult())
            .AddEndpointFilter<ValidationFilter<UpdateSupplierBody>>()
            .WithName("UpdateSupplier")
            .WithSummary("Update supplier");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteSupplier.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteSupplier.Command(id), ct)).ToNoContentResult())
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

        group.MapGet("/stats", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var stats = new
            {
                TotalSuppliers = await context.Suppliers.CountAsync(s => !s.IsDeleted, ct),
                ActiveSuppliers = await context.Suppliers.CountAsync(s => !s.IsDeleted && s.IsActive, ct)
            };

            return Results.Ok(new { data = stats });
        })
        .WithName("GetSupplierStats")
        .WithSummary("Get supplier statistics");
    }
}

public record UpdateSupplierBody(
    string? Name, string? Phone, string? CompanyName, string? ContactPerson,
    string? AlternatePhone, string? Email, string? AddressLine1, string? AddressLine2,
    string? City, string? State, string? PostalCode, string? Country,
    string? SupplierType, string? TaxRegistrationNo, string? PaymentTerms,
    int? LeadTimeDays, string? Notes, bool? IsActive);
