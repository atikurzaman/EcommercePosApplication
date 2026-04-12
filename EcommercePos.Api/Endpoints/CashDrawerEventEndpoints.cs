using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class CashDrawerEventEndpoints
{
    public static void MapCashDrawerEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cash-drawer-events").WithTags("Cash Drawer Events");

        group.MapGet("/", async (
            [FromQuery] Guid cashShiftId,
            [FromServices] GetCashDrawerEvents.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCashDrawerEvents.Request(cashShiftId), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCashDrawerEvents")
        .WithSummary("Get cash drawer events for a shift");

        group.MapPost("/", async (
            [FromBody] RecordCashDrawerEvent.Request request,
            [FromServices] RecordCashDrawerEvent.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/cash-drawer-events/{result.Value?.Id}");
        })
        .WithName("RecordCashDrawerEvent")
        .WithSummary("Record a cash drawer event");
    }
}
