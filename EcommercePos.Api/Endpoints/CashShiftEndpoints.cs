using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class CashShiftEndpoints
{
    public static void MapCashShiftEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cash-shifts").WithTags("Cash Shifts");

        group.MapGet("/", async (
            [AsParameters] GetCashShifts.Request request,
            [FromServices] GetCashShifts.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetCashShifts")
        .WithSummary("Get paginated cash shifts with filters");

        group.MapGet("/active", async (
            [FromQuery] Guid? userId,
            [FromQuery] Guid? warehouseId,
            [FromServices] GetActiveShift.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetActiveShift.Query(userId, warehouseId), ct);
            return result.ToHttpResult();
        })
        .WithName("GetActiveShift")
        .WithSummary("Get the currently active shift");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetShiftSummary.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetShiftSummary.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetShiftSummary")
        .WithSummary("Get shift summary with transactions and events");

        group.MapPost("/open", async (
            [FromBody] OpenShift.Request request,
            [FromServices] OpenShift.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/cash-shifts/{result.Value?.Id}");
        })
        .WithName("OpenShift")
        .WithSummary("Open a new cash shift");

        group.MapPost("/{id:guid}/close", async (
            Guid id,
            [FromBody] CloseShift.Command request,
            [FromServices] CloseShift.Handler handler,
            CancellationToken ct) =>
        {
            var command = request with { ShiftId = id };
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("CloseShift")
        .WithSummary("Close an open cash shift");
    }
}
