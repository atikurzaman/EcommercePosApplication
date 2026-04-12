using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Security;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles").WithTags("Roles");

        group.MapGet("/", async (
            [AsParameters] GetRoles.Request request,
            [FromServices] GetRoles.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetRoles")
        .WithSummary("Get paginated roles");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetRoleById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetRoleById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetRoleById")
        .WithSummary("Get role by id with permissions and menus");

        group.MapPost("/", async (
            [FromBody] CreateRole.Request request,
            [FromServices] CreateRole.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/roles");
        })
        .WithName("CreateRole")
        .WithSummary("Create a new role");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateRole.Request request,
            [FromServices] UpdateRole.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateRole.Command(id, request.Name, request.Description, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateRole")
        .WithSummary("Update an existing role");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteRole.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteRole.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteRole")
        .WithSummary("Delete a role");

        group.MapPut("/{id:guid}/permissions", async (
            Guid id,
            [FromBody] AssignPermissionsRequest request,
            [FromServices] AssignPermissionsToRole.Handler handler,
            CancellationToken ct) =>
        {
            var command = new AssignPermissionsToRole.Command(id, request.Permissions);
            var result = await handler.Handle(command, ct);
            return result.ToNoContentResult();
        })
        .WithName("AssignPermissionsToRole")
        .WithSummary("Assign permissions to a role");

        group.MapPut("/{id:guid}/menus", async (
            Guid id,
            [FromBody] AssignMenusRequest request,
            [FromServices] AssignMenusToRole.Handler handler,
            CancellationToken ct) =>
        {
            var command = new AssignMenusToRole.Command(id, request.Menus);
            var result = await handler.Handle(command, ct);
            return result.ToNoContentResult();
        })
        .WithName("AssignMenusToRole")
        .WithSummary("Assign menus to a role");
    }
}

record AssignPermissionsRequest(List<AssignPermissionsToRole.PermissionAssignment> Permissions);
record AssignMenusRequest(List<AssignMenusToRole.MenuAssignment> Menus);
