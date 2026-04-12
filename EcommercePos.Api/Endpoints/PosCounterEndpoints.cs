using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class PosCounterEndpoints
{
    public static void MapPosCounterEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pos-counters").WithTags("POS Counters");

        group.MapGet("/", async (
            [AsParameters] GetPosCounters.Request request,
            [FromServices] GetPosCounters.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetPosCounters")
        .WithSummary("Get paginated POS counters filtered by warehouse");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetPosCounterById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetPosCounterById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPosCounterById")
        .WithSummary("Get POS counter by id with terminals");

        group.MapPost("/", async (
            [FromBody] CreatePosCounter.Request request,
            [FromServices] CreatePosCounter.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/pos-counters/{result.Value?.Id}");
        })
        .WithName("CreatePosCounter")
        .WithSummary("Create a new POS counter");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdatePosCounter.Request request,
            [FromServices] UpdatePosCounter.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdatePosCounter.Command(
                id, request.CounterCode, request.CounterName, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdatePosCounter")
        .WithSummary("Update an existing POS counter");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeletePosCounter.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeletePosCounter.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeletePosCounter")
        .WithSummary("Soft delete a POS counter");
    }
}
