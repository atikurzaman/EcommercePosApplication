using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class ProductConditionEndpoints
{
    public static void MapProductConditionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product-conditions").WithTags("ProductConditions");

        group.MapGet("/", async (
            [AsParameters] GetProductConditions.Request request,
            [FromServices] GetProductConditions.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetProductConditions")
        .WithSummary("Get paginated product conditions");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetProductConditionByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetProductConditionByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductConditionByCode")
        .WithSummary("Get product condition by code");

        group.MapPost("/", async (
            [FromBody] CreateProductCondition.Request request,
            [FromServices] CreateProductCondition.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/product-conditions/{request.ConditionCode}");
        })
        .WithName("CreateProductCondition")
        .WithSummary("Create a new product condition");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateProductCondition.Request request,
            [FromServices] UpdateProductCondition.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateProductCondition.Command(code, request.ConditionCode, request.DisplayName);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateProductCondition")
        .WithSummary("Update an existing product condition");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteProductCondition.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteProductCondition.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteProductCondition")
        .WithSummary("Delete a product condition");
    }
}
