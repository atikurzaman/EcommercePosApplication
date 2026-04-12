using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Catalog;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class CollectionEndpoints
{
    public static void MapCollectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/collections").WithTags("Collections");

        group.MapGet("/", async (
            [AsParameters] GetCollections.Request request,
            [FromServices] GetCollections.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetCollections")
        .WithSummary("Get active collections");

        group.MapGet("/home", async (
            [FromServices] GetHomePageCollections.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(ct);
            return result.ToHttpResult();
        })
        .WithName("GetHomePageCollections")
        .WithSummary("Get collections for home page");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetCollectionById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCollectionById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCollectionById")
        .WithSummary("Get collection with products");
    }
}
