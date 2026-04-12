using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Cart;
using EcommercePos.Api.Extensions;
using EcommercePos.Shared.Common;
using MediatR;

namespace EcommercePos.Api.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts").WithTags("Cart");

        group.MapGet("/", async (
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
            (await mediator.Send(new GetCartsQuery(), ct)).ToHttpResult())
        .WithName("GetCarts")
        .WithSummary("Get all carts");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
            (await mediator.Send(new GetCartByIdQuery(id), ct)).ToHttpResult())
        .WithName("GetCartById")
        .WithSummary("Get cart by id");

        group.MapPost("/", async (
            [FromBody] CreateCartCommand command,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
            (await mediator.Send(command, ct)).ToCreatedResult("/api/carts"))
        .WithName("CreateCart")
        .WithSummary("Create a new cart");

        group.MapPost("/items", async (
            [FromBody] AddCartItemCommand command,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
            (await mediator.Send(command, ct)).ToHttpResult())
        .WithName("AddCartItem")
        .WithSummary("Add item to cart");

        group.MapPut("/items/{itemId:guid}", async (
            Guid itemId,
            [FromBody] decimal quantity,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
            (await mediator.Send(new UpdateCartItemCommand(itemId, quantity), ct)).ToHttpResult())
        .WithName("UpdateCartItem")
        .WithSummary("Update cart item quantity");

        group.MapDelete("/items/{itemId:guid}", async (
            Guid itemId,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
            (await mediator.Send(new RemoveCartItemCommand(itemId), ct)).ToNoContentResult())
        .WithName("RemoveCartItem")
        .WithSummary("Remove item from cart");

        group.MapPost("/apply-coupon", async (
            [FromBody] ApplyCouponCommand command,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
            (await mediator.Send(command, ct)).ToHttpResult())
        .WithName("ApplyCoupon")
        .WithSummary("Apply coupon to cart");
    }
}
