using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Catalog;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tags").WithTags("Tags");

        group.MapGet("/", async (
            [AsParameters] GetTags.Request request,
            [FromServices] GetTags.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetTags.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToPagedResult();
        })
        .WithName("GetTags")
        .WithSummary("Get paginated tags with product count");

        group.MapPost("/", async (
            [FromBody] CreateTag.Request request,
            [FromServices] CreateTag.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateTag.Command(request.Name, request.Slug);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult("/api/tags");
        })
        .WithName("CreateTag")
        .WithSummary("Create a new tag");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateTag.Request body,
            [FromServices] UpdateTag.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateTag.Command(id, body.Name, body.Slug);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateTag")
        .WithSummary("Update an existing tag");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteTag.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteTag.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteTag")
        .WithSummary("Soft delete a tag");
    }
}
