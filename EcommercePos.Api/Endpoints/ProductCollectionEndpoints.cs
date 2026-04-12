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
            [FromBody] UpdateCollectionBody body,
            [FromServices] UpdateCollection.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateCollection.Command(
                id, body.Name, body.Slug, body.Description, body.ImageUrl,
                body.DisplayOrder, body.IsActive, body.ShowInHomePage);
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
            [FromBody] ManageCollectionItemsBody body,
            [FromServices] ManageCollectionItems.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ManageCollectionItems.Command(id, body.Items);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ManageCollectionItems")
        .WithSummary("Manage collection items");
    }
}

record UpdateCollectionBody(
    string Name, string? Slug, string? Description, string? ImageUrl,
    int DisplayOrder, bool IsActive, bool ShowInHomePage);

record ManageCollectionItemsBody(
    List<ManageCollectionItems.CollectionItemInput> Items);
