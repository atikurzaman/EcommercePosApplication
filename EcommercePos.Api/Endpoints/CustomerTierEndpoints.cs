using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class CustomerTierEndpoints
{
    public static void MapCustomerTierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customer-tiers").WithTags("CustomerTiers");

        group.MapGet("/", async (
            [AsParameters] GetCustomerTiers.Request request,
            [FromServices] GetCustomerTiers.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetCustomerTiers")
        .WithSummary("Get paginated customer tiers");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetCustomerTierByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCustomerTierByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCustomerTierByCode")
        .WithSummary("Get customer tier by code");

        group.MapPost("/", async (
            [FromBody] CreateCustomerTier.Request request,
            [FromServices] CreateCustomerTier.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/customer-tiers/{request.TierCode}");
        })
        .AddEndpointFilter<ValidationFilter<CreateCustomerTier.Request>>()
        .WithName("CreateCustomerTier")
        .WithSummary("Create a new customer tier");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateCustomerTier.Request request,
            [FromServices] UpdateCustomerTier.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateCustomerTier.Command(code, request.TierCode, request.DisplayName, request.MinLifetimeSpend, request.DiscountPct, request.PointsMultiplier, request.SortOrder);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .AddEndpointFilter<ValidationFilter<UpdateCustomerTier.Request>>()
        .WithName("UpdateCustomerTier")
        .WithSummary("Update an existing customer tier");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteCustomerTier.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteCustomerTier.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteCustomerTier")
        .WithSummary("Delete a customer tier");
    }
}
