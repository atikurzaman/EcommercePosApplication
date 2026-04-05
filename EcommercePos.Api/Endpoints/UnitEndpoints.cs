using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Unit.Queries;
using EcommercePos.Application.Features.Unit.Commands;
using EcommercePos.Api.Extensions;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class UnitEndpoints
{
    public static void MapUnitEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/units").WithTags("Units");

        group.MapGet("/", async (
            [AsParameters] GetUnits.Request request,
            [FromServices] GetUnits.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetUnits.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetUnits")
        .WithSummary("Get paginated units");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetUnitById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetUnitById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetUnitById")
        .WithSummary("Get unit by id");

        group.MapPost("/", async (
            [FromBody] CreateUnit.Request request,
            [FromServices] CreateUnit.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateUnit.Command(
                request.ShortName, request.Name, request.Description,
                request.BaseUnitId, request.ConversionFactor, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/units/{request.ShortName}");
        })
        .WithName("CreateUnit")
        .WithSummary("Create a new unit");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateUnit.Request request,
            [FromServices] UpdateUnit.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateUnit.Command(
                id, request.ShortName, request.Name, request.Description,
                request.BaseUnitId, request.ConversionFactor, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateUnit")
        .WithSummary("Update an existing unit");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteUnit.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteUnit.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteUnit")
        .WithSummary("Soft delete a unit");
    }
}