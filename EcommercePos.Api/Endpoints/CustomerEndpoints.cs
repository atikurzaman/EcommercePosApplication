using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Customer.Queries;
using EcommercePos.Application.Features.Customer.Commands;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", async (
            [AsParameters] GetCustomers.Request request,
            [FromServices] GetCustomers.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetCustomers.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetCustomers")
        .WithSummary("Get paginated customers");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetCustomerById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCustomerById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCustomerById")
        .WithSummary("Get customer by id");

        group.MapPost("/", async (
            [FromBody] CreateCustomer.Request request,
            [FromServices] CreateCustomer.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateCustomer.Command(
                request.CustomerCode, request.CustomerType, request.Phone,
                request.AlternatePhone, request.Email, request.DateOfBirth,
                request.Gender, request.CompanyName, request.TaxNumber,
                request.AddressLine1, request.City, request.Country,
                request.CreditLimit, request.CustomerGroup, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/customers/{command.CustomerCode}");
        })
        .WithName("CreateCustomer")
        .WithSummary("Create a new customer");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateCustomer.Request request,
            [FromServices] UpdateCustomer.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateCustomer.Command(
                id, request.CustomerCode, request.CustomerType, request.Phone,
                request.AlternatePhone, request.Email, request.DateOfBirth,
                request.Gender, request.CompanyName, request.TaxNumber,
                request.AddressLine1, request.City, request.Country,
                request.CreditLimit, request.CustomerGroup, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateCustomer")
        .WithSummary("Update an existing customer");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteCustomer.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteCustomer.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteCustomer")
        .WithSummary("Soft delete a customer");
    }
}