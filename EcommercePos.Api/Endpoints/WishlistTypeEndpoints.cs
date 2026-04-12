using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class WishlistTypeEndpoints
{
    public static void MapWishlistTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wishlist-types").WithTags("WishlistTypes");

        group.MapGet("/", async (
            [AsParameters] GetWishlistTypes.Request request,
            [FromServices] GetWishlistTypes.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetWishlistTypes")
        .WithSummary("Get paginated wishlist types");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetWishlistTypeByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetWishlistTypeByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetWishlistTypeByCode")
        .WithSummary("Get wishlist type by code");

        group.MapPost("/", async (
            [FromBody] CreateWishlistType.Request request,
            [FromServices] CreateWishlistType.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/wishlist-types/{request.TypeCode}");
        })
        .AddEndpointFilter<ValidationFilter<CreateWishlistType.Request>>()
        .WithName("CreateWishlistType")
        .WithSummary("Create a new wishlist type");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateWishlistType.Request request,
            [FromServices] UpdateWishlistType.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateWishlistType.Command(code, request.TypeCode, request.DisplayName);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .AddEndpointFilter<ValidationFilter<UpdateWishlistType.Request>>()
        .WithName("UpdateWishlistType")
        .WithSummary("Update an existing wishlist type");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteWishlistType.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteWishlistType.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteWishlistType")
        .WithSummary("Delete a wishlist type");
    }
}
