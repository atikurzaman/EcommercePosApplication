using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Security;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class PermissionEndpoints
{
    public static void MapPermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/permissions").WithTags("Permissions");

        group.MapGet("/", async (
            [AsParameters] GetPermissions.Request request,
            [FromServices] GetPermissions.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetPermissions")
        .WithSummary("Get paginated permissions");

        group.MapGet("/modules", async (
            [FromServices] GetPermissionModules.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(ct);
            return result.ToHttpResult();
        })
        .WithName("GetPermissionModules")
        .WithSummary("Get distinct permission module list");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetPermissionById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetPermissionById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPermissionById")
        .WithSummary("Get permission by id");

        group.MapPost("/", async (
            [FromBody] CreatePermission.Request request,
            [FromServices] CreatePermission.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/permissions");
        })
        .WithName("CreatePermission")
        .WithSummary("Create a new permission");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdatePermission.Request request,
            [FromServices] UpdatePermission.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdatePermission.Command(
                id, request.PermissionCode, request.Name, request.Module,
                request.Description, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdatePermission")
        .WithSummary("Update an existing permission");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeletePermission.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeletePermission.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeletePermission")
        .WithSummary("Soft delete a permission");
    }
}
