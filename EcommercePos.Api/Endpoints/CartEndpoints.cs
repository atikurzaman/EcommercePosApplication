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
        {
            var result = await mediator.Send(new GetCartsQuery(), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCarts")
        .WithSummary("Get all carts");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCartByIdQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCartById")
        .WithSummary("Get cart by id");

        group.MapPost("/", async (
            [FromBody] CreateCartCommand command,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.ToCreatedResult($"/api/carts/{result.Value?.Id}");
        })
        .WithName("CreateCart")
        .WithSummary("Create a new cart");

        group.MapPost("/items", async (
            [FromBody] AddCartItemCommand command,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("AddCartItem")
        .WithSummary("Add item to cart");

        group.MapPut("/items/{itemId:guid}", async (
            Guid itemId,
            [FromBody] decimal quantity,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new UpdateCartItemCommand(itemId, quantity);
            var result = await mediator.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateCartItem")
        .WithSummary("Update cart item quantity");

        group.MapDelete("/items/{itemId:guid}", async (
            Guid itemId,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new RemoveCartItemCommand(itemId), ct);
            return result.ToNoContentResult();
        })
        .WithName("RemoveCartItem")
        .WithSummary("Remove item from cart");

        group.MapPost("/apply-coupon", async (
            [FromBody] ApplyCouponCommand command,
            [FromServices] IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ApplyCoupon")
        .WithSummary("Apply coupon to cart");
    }
}
