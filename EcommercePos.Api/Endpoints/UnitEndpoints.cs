using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Unit;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

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
            .AddEndpointFilter<ValidationFilter<CreateUnit.Command>>()
            .WithName("CreateUnit")
            .WithSummary("Create a new unit");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateUnit.Command body,
            UpdateUnit.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { Id = id }, ct)).ToHttpResult())
            .AddEndpointFilter<ValidationFilter<UpdateUnit.Command>>()
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
