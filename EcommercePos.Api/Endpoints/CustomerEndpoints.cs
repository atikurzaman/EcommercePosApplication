using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Customer;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

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
            .AddEndpointFilter<ValidationFilter<CreateCustomer.Command>>()
            .WithName("CreateCustomer")
            .WithSummary("Create a new customer");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateCustomer.Command body,
            UpdateCustomer.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { Id = id }, ct)).ToHttpResult())
            .AddEndpointFilter<ValidationFilter<UpdateCustomer.Command>>()
            .WithName("UpdateCustomer")
            .WithSummary("Update customer");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteCustomer.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteCustomer.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteCustomer")
            .WithSummary("Soft delete customer");

        group.MapPost("/{id:guid}/toggle-active", async (
            Guid id,
            ToggleCustomerActive.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ToggleCustomerActive.Command(id), ct)).ToHttpResult())
            .WithName("ToggleCustomerActive")
            .WithSummary("Toggle customer active status");

        group.MapGet("/stats", async (
            GetCustomerStats.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetCustomerStats.Query(), ct)).ToHttpResult())
            .WithName("GetCustomerStats")
            .WithSummary("Get customer statistics");

        group.MapGet("/addresses/{customerId:guid}", async (
            Guid customerId,
            GetCustomerAddresses.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetCustomerAddresses.Query(customerId), ct)).ToHttpResult())
            .WithName("GetCustomerAddresses")
            .WithSummary("Get customer addresses");
    }
}
