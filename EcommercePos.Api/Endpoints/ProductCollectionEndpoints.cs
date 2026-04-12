using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Catalog;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class ProductCollectionEndpoints
{
    public static void MapProductCollectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product-collections").WithTags("ProductCollections");

        group.MapGet("/", async (
            [AsParameters] GetCollections.Request request,
            [FromServices] GetCollections.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetProductCollections")
        .WithSummary("Get paginated product collections");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetCollectionById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCollectionById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProductCollectionById")
        .WithSummary("Get product collection by id with products");

        group.MapPost("/", async (
            [FromBody] CreateCollection.Request request,
            [FromServices] CreateCollection.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult("/api/product-collections");
        })
        .WithName("CreateProductCollection")
        .WithSummary("Create a new product collection");

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
        .WithName("UpdateProductCollection")
        .WithSummary("Update an existing product collection");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteCollection.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteCollection.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteProductCollection")
        .WithSummary("Soft delete a product collection");

        group.MapPut("/{id:guid}/items", async (
            Guid id,
            [FromBody] List<ManageCollectionItems.CollectionItemInput> items,
            [FromServices] ManageCollectionItems.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ManageCollectionItems.Command(id, items);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ManageCollectionItems")
        .WithSummary("Manage collection items");
    }
}
