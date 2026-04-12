using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class ColorEndpoints
{
    public static void MapColorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/colors").WithTags("Colors");

        group.MapGet("/", async (
            [AsParameters] GetColors.Request request,
            [FromServices] GetColors.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetColors")
        .WithSummary("Get paginated colors");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetColorById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetColorById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetColorById")
        .WithSummary("Get color by id");

        group.MapPost("/", async (
            [FromBody] CreateColor.Request request,
            [FromServices] CreateColor.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult("/api/colors");
        })
        .WithName("CreateColor")
        .WithSummary("Create a new color");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateColor.Request request,
            [FromServices] UpdateColor.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateColor.Command(id, request.Name, request.HexCode, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateColor")
        .WithSummary("Update an existing color");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteColor.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteColor.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteColor")
        .WithSummary("Soft delete a color");
    }
}
