using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Unit;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class UnitEndpoints
{
    public static void MapUnitEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/units").WithTags("Units");

        group.MapGet("/", async (
            [AsParameters] GetUnits.Query query,
            GetUnits.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetUnits")
            .WithSummary("Get paginated units");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetUnitById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetUnitById.Query(id), ct)).ToHttpResult())
            .WithName("GetUnitById")
            .WithSummary("Get unit by id");

        group.MapPost("/", async (
            [FromBody] CreateUnit.Command command,
            CreateUnit.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/units"))
            .WithName("CreateUnit")
            .WithSummary("Create a new unit");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateUnitBody body,
            UpdateUnit.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateUnit.Command(
                id, body.ShortName, body.Name, body.Description,
                body.BaseUnitId, body.ConversionFactor, body.IsActive), ct)).ToHttpResult())
            .WithName("UpdateUnit")
            .WithSummary("Update an existing unit");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteUnit.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteUnit.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteUnit")
            .WithSummary("Soft delete a unit");
    }
}

public record UpdateUnitBody(
    string ShortName, string Name, string? Description,
    Guid? BaseUnitId, decimal? ConversionFactor, bool IsActive);
