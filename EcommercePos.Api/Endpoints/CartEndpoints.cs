using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Cart;
using EcommercePos.Api.Extensions;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts").WithTags("Cart");

        group.MapGet("/", async (
            [AsParameters] GetCarts.Request request,
            [FromServices] GetCarts.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetCarts.Query(request.PageIndex, request.PageSize, request.CustomerId);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetCarts")
        .WithSummary("Get paginated carts");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetCartById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCartById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCartById")
        .WithSummary("Get cart by id");

        group.MapPost("/", async (
            [FromBody] CreateCart.Request request,
            [FromServices] CreateCart.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateCart.Command(request);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/carts/{result.Value?.Id}");
        })
        .WithName("CreateCart")
        .WithSummary("Create a new cart");

        group.MapPost("/items", async (
            [FromBody] AddCartItem.Request request,
            [FromServices] AddCartItem.Handler handler,
            CancellationToken ct) =>
        {
            var command = new AddCartItem.Command(request);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("AddCartItem")
        .WithSummary("Add item to cart");

        group.MapPut("/items/{itemId:guid}", async (
            Guid itemId,
            [FromBody] UpdateCartItem.Request request,
            [FromServices] UpdateCartItem.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateCartItem.Command(new UpdateCartItem.Request(itemId, request.Quantity));
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateCartItem")
        .WithSummary("Update cart item quantity");

        group.MapDelete("/items/{itemId:guid}", async (
            Guid itemId,
            [FromServices] RemoveCartItem.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new RemoveCartItem.Command(itemId), ct);
            return result.ToNoContentResult();
        })
        .WithName("RemoveCartItem")
        .WithSummary("Remove item from cart");

        group.MapPost("/apply-coupon", async (
            [FromBody] ApplyCoupon.Request request,
            [FromServices] ApplyCoupon.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ApplyCoupon.Command(request);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ApplyCoupon")
        .WithSummary("Apply coupon to cart");
    }
}
