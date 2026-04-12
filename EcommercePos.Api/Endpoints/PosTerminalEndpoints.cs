using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class PosTerminalEndpoints
{
    public static void MapPosTerminalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pos-terminals").WithTags("POS Terminals");

        group.MapGet("/", async (
            [AsParameters] GetPosTerminals.Request request,
            [FromServices] GetPosTerminals.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetPosTerminals")
        .WithSummary("Get paginated POS terminals filtered by counter");

        group.MapPost("/", async (
            [FromBody] CreatePosTerminal.Request request,
            [FromServices] CreatePosTerminal.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/pos-terminals/{result.Value?.Id}");
        })
        .WithName("CreatePosTerminal")
        .WithSummary("Create a new POS terminal");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdatePosTerminal.Request request,
            [FromServices] UpdatePosTerminal.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdatePosTerminal.Command(
                id, request.TerminalCode, request.TerminalName,
                request.MachineName, request.Ipaddress, request.PrinterName, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdatePosTerminal")
        .WithSummary("Update an existing POS terminal");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeletePosTerminal.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeletePosTerminal.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeletePosTerminal")
        .WithSummary("Soft delete a POS terminal");
    }
}
