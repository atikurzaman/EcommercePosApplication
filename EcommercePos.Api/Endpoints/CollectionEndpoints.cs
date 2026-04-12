using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Catalog;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

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

        group.MapPost("/", async (
            [FromBody] CreateCollection.Request request,
            [FromServices] CreateCollection.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/collections");
        })
        .AddEndpointFilter<ValidationFilter<CreateCollection.Request>>()
        .WithName("CreateCollection")
        .WithSummary("Create a new collection");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateCollection.Command body,
            [FromServices] UpdateCollection.Handler handler,
            CancellationToken ct) =>
        {
            var command = body with { Id = id };
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .AddEndpointFilter<ValidationFilter<UpdateCollection.Command>>()
        .WithName("UpdateCollection")
        .WithSummary("Update a collection");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteCollection.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteCollection.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteCollection")
        .WithSummary("Delete a collection");

        group.MapPut("/{id:guid}/items", async (
            Guid id,
            [FromBody] ManageCollectionItems.Command body,
            [FromServices] ManageCollectionItems.Handler handler,
            CancellationToken ct) =>
        {
            var command = body with { CollectionId = id };
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateCollectionItems")
        .WithSummary("Set products in a collection");
    }
}
