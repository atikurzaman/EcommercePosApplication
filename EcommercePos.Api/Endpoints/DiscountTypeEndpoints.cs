using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class DiscountTypeEndpoints
{
    public static void MapDiscountTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/discount-types").WithTags("DiscountTypes");

        group.MapGet("/", async (
            [AsParameters] GetDiscountTypes.Request request,
            [FromServices] GetDiscountTypes.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetDiscountTypes")
        .WithSummary("Get paginated discount types");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetDiscountTypeByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetDiscountTypeByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetDiscountTypeByCode")
        .WithSummary("Get discount type by code");

        group.MapPost("/", async (
            [FromBody] CreateDiscountType.Request request,
            [FromServices] CreateDiscountType.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/discount-types/{request.TypeCode}");
        })
        .AddEndpointFilter<ValidationFilter<CreateDiscountType.Request>>()
        .WithName("CreateDiscountType")
        .WithSummary("Create a new discount type");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateDiscountType.Request request,
            [FromServices] UpdateDiscountType.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateDiscountType.Command(code, request.TypeCode, request.DisplayName);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .AddEndpointFilter<ValidationFilter<UpdateDiscountType.Request>>()
        .WithName("UpdateDiscountType")
        .WithSummary("Update an existing discount type");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteDiscountType.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteDiscountType.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteDiscountType")
        .WithSummary("Delete a discount type");
    }
}
