using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class PosReturnEndpoints
{
    public static void MapPosReturnEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pos-returns").WithTags("POS Returns");

        group.MapGet("/", async (
            [AsParameters] GetPosReturns.Request request,
            [FromServices] GetPosReturns.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetPosReturns.Query(
                request.PageIndex, request.PageSize,
                request.WarehouseId, request.DateFrom, request.DateTo);
            var result = await handler.Handle(query, ct);
            return result.ToPagedResult();
        })
        .WithName("GetPosReturns")
        .WithSummary("Get paginated POS returns");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetPosReturnById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetPosReturnById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPosReturnById")
        .WithSummary("Get POS return by id");

        group.MapPost("/", async (
            [FromBody] ProcessPosReturn.Request request,
            [FromServices] ProcessPosReturn.Handler handler,
            CancellationToken ct) =>
        {
            var command = new ProcessPosReturn.Command(request);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/pos-returns/{result.Value?.Id}");
        })
        .WithName("ProcessPosReturn")
        .WithSummary("Process a POS return");
    }
}
