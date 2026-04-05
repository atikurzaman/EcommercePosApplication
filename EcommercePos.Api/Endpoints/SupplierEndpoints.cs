using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Supplier.Queries;
using EcommercePos.Application.Features.Supplier.Commands;
using EcommercePos.Api.Extensions;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class SupplierEndpoints
{
    public static void MapSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suppliers").WithTags("Suppliers");

        group.MapGet("/", async (
            [AsParameters] GetSuppliers.Request request,
            [FromServices] GetSuppliers.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetSuppliers.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetSuppliers")
        .WithSummary("Get paginated suppliers");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetSupplierById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetSupplierById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetSupplierById")
        .WithSummary("Get supplier by id");

        group.MapPost("/", async (
            [FromBody] CreateSupplier.Request request,
            [FromServices] CreateSupplier.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateSupplier.Command(
                request.SupplierCode, request.Name, request.CompanyName, request.ContactPerson,
                request.Phone, request.AlternatePhone, request.Email, request.AddressLine1,
                request.AddressLine2, request.City, request.State, request.PostalCode,
                string.IsNullOrEmpty(request.Country) ? "Thailand" : request.Country,
                request.SupplierType, request.TaxRegistrationNo, request.PaymentTerms,
                request.LeadTimeDays, request.Notes, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/suppliers/{request.Name}");
        })
        .WithName("CreateSupplier")
        .WithSummary("Create a new supplier");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateSupplier.Request request,
            [FromServices] UpdateSupplier.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateSupplier.Command(
                id, request.SupplierCode, request.Name, request.CompanyName, request.ContactPerson,
                request.Phone, request.AlternatePhone, request.Email, request.AddressLine1,
                request.AddressLine2, request.City, request.State, request.PostalCode,
                string.IsNullOrEmpty(request.Country) ? "Thailand" : request.Country,
                request.SupplierType, request.TaxRegistrationNo, request.PaymentTerms,
                request.LeadTimeDays, request.Notes, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateSupplier")
        .WithSummary("Update an existing supplier");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteSupplier.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteSupplier.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteSupplier")
        .WithSummary("Soft delete a supplier");
    }
}
