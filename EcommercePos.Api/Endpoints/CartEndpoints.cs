using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Cart;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts").WithTags("Carts");

        group.MapGet("/", async (
            [AsParameters] GetCarts.Query query,
            GetCarts.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetCarts")
            .WithSummary("Get paginated carts");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetCartById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetCartById.Query(id), ct)).ToHttpResult())
            .WithName("GetCartById")
            .WithSummary("Get cart by id");

        group.MapPost("/", async (
            [FromBody] CreateCart.Command command,
            CreateCart.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/carts"))
            .WithName("CreateCart")
            .WithSummary("Create a new cart");

        group.MapPost("/items", async (
            [FromBody] AddCartItem.Command command,
            AddCartItem.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/carts/items"))
            .WithName("AddCartItem")
            .WithSummary("Add item to cart");

        group.MapPut("/items/{itemId:guid}", async (
            Guid itemId,
            [FromBody] UpdateCartItem.Command body,
            UpdateCartItem.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { ItemId = itemId }, ct)).ToHttpResult())
            .WithName("UpdateCartItem")
            .WithSummary("Update cart item quantity");

        group.MapDelete("/items/{itemId:guid}", async (
            Guid itemId,
            RemoveCartItem.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new RemoveCartItem.Command(itemId), ct)).ToNoContentResult())
            .WithName("RemoveCartItem")
            .WithSummary("Remove item from cart");

        group.MapPost("/apply-coupon", async (
            [FromBody] ApplyCoupon.Command command,
            ApplyCoupon.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToHttpResult())
            .WithName("ApplyCoupon")
            .WithSummary("Apply coupon to cart");
    }
}
