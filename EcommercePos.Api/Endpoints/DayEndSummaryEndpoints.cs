using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Pos;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class DayEndSummaryEndpoints
{
    public static void MapDayEndSummaryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/day-end-summaries").WithTags("Day-End Summaries");

        group.MapGet("/", async (
            [AsParameters] GetDayEndSummaries.Request request,
            [FromServices] GetDayEndSummaries.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetDayEndSummaries")
        .WithSummary("Get paginated day-end summaries");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetDayEndSummaryById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetDayEndSummaryById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetDayEndSummaryById")
        .WithSummary("Get day-end summary by id");

        group.MapPost("/generate", async (
            [FromBody] GenerateDayEndSummary.Command command,
            [FromServices] GenerateDayEndSummary.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/day-end-summaries/{result.Value?.Id}");
        })
        .WithName("GenerateDayEndSummary")
        .WithSummary("Generate a day-end summary for a warehouse");
    }
}
