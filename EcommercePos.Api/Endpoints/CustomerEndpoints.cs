using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Customer;
using EcommercePos.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", async (
            [AsParameters] GetCustomers.Query query,
            GetCustomers.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetCustomers")
            .WithSummary("Get paginated customers");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetCustomerById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetCustomerById.Query(id), ct)).ToHttpResult())
            .WithName("GetCustomerById")
            .WithSummary("Get customer with details");

        group.MapPost("/", async (
            [FromBody] CreateCustomer.Command command,
            CreateCustomer.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/customers"))
            .WithName("CreateCustomer")
            .WithSummary("Create a new customer");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateCustomerBody body,
            UpdateCustomer.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateCustomer.Command(
                id, body.Phone, body.AlternatePhone, body.Email, body.DateOfBirth,
                body.Gender, body.CompanyName, body.TaxNumber, body.AddressLine1,
                body.City, body.Country, body.CreditLimit, body.IsActive), ct)).ToHttpResult())
            .WithName("UpdateCustomer")
            .WithSummary("Update customer");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteCustomer.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteCustomer.Command(id), ct)).ToNoContentResult())
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
                TotalCustomers = await context.Customers.CountAsync(c => !c.IsDeleted, ct),
                ActiveCustomers = await context.Customers.CountAsync(c => !c.IsDeleted && c.IsActive, ct),
                NewCustomersToday = await context.Customers.CountAsync(c => !c.IsDeleted && c.RegistrationDate >= today, ct),
                TotalLoyaltyPoints = await context.Customers.Where(c => !c.IsDeleted).SumAsync(c => c.LoyaltyPoints, ct)
            };

            return Results.Ok(new { data = stats });
        })
        .WithName("GetCustomerStats")
        .WithSummary("Get customer statistics");

        group.MapGet("/addresses/{customerId:guid}", async (Guid customerId, ApplicationDbContext context, CancellationToken ct) =>
        {
            var addresses = await context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && !a.IsDeleted)
                .OrderByDescending(a => a.IsDefault).ThenBy(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id, a.AddressType, a.Label, a.FullName, a.PhoneNumber,
                    a.AddressLine1, a.AddressLine2, a.City, a.State, a.PostalCode, a.IsDefault
                })
                .ToListAsync(ct);

            return Results.Ok(new { data = addresses });
        })
        .WithName("GetCustomerAddresses")
        .WithSummary("Get customer addresses");
    }
}

public record UpdateCustomerBody(
    string? Phone, string? AlternatePhone, string? Email,
    DateTime? DateOfBirth, string? Gender, string? CompanyName, string? TaxNumber,
    string? AddressLine1, string? City, string? Country, decimal? CreditLimit, bool IsActive);
