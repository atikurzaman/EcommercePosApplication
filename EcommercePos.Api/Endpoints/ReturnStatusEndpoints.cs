using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class ReturnStatusEndpoints
{
    public static void MapReturnStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/return-statuses").WithTags("ReturnStatuses");

        group.MapGet("/", async (
            [AsParameters] GetReturnStatuses.Request request,
            [FromServices] GetReturnStatuses.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetReturnStatuses")
        .WithSummary("Get paginated return statuses");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetReturnStatusByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetReturnStatusByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetReturnStatusByCode")
        .WithSummary("Get return status by code");

        group.MapPost("/", async (
            [FromBody] CreateReturnStatus.Request request,
            [FromServices] CreateReturnStatus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/return-statuses/{request.StatusCode}");
        })
        .WithName("CreateReturnStatus")
        .WithSummary("Create a new return status");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateReturnStatus.Request request,
            [FromServices] UpdateReturnStatus.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateReturnStatus.Command(code, request.StatusCode, request.DisplayName, request.SortOrder);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateReturnStatus")
        .WithSummary("Update an existing return status");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteReturnStatus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteReturnStatus.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteReturnStatus")
        .WithSummary("Delete a return status");
    }
}
