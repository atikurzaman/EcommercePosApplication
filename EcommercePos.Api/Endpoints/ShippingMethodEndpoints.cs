using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.ShippingMethod;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class ShippingMethodEndpoints
{
    public static void MapShippingMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/shipping-methods").WithTags("ShippingMethods");

        group.MapGet("/", async (
            [AsParameters] GetShippingMethods.Query request,
            GetShippingMethods.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetShippingMethods")
        .WithSummary("Get paginated shipping methods");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetShippingMethodById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetShippingMethodById.Query(id), ct)).ToHttpResult())
        .WithName("GetShippingMethodById")
        .WithSummary("Get shipping method by id");

        group.MapPost("/", async (
            [FromBody] CreateShippingMethod.Command command,
            CreateShippingMethod.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/shipping-methods"))
        .AddEndpointFilter<ValidationFilter<CreateShippingMethod.Command>>()
        .WithName("CreateShippingMethod")
        .WithSummary("Create a new shipping method");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateShippingMethod.Command command,
            UpdateShippingMethod.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateShippingMethod.Command(
                id,
                command.Name,
                command.Description,
                command.CarrierName,
                command.BaseCost,
                command.CostPerKg,
                command.EstimatedDaysMin,
                command.EstimatedDaysMax,
                command.IsActive,
                command.IsFreeShipping,
                command.FreeShippingThreshold,
                command.DisplayOrder), ct)).ToHttpResult())
        .AddEndpointFilter<ValidationFilter<UpdateShippingMethod.Command>>()
        .WithName("UpdateShippingMethod")
        .WithSummary("Update an existing shipping method");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteShippingMethod.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteShippingMethod.Command(id), ct)).ToNoContentResult())
        .WithName("DeleteShippingMethod")
        .WithSummary("Soft delete a shipping method");
    }
}
