using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Security;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", async (
            [AsParameters] GetUsers.Request request,
            [FromServices] GetUsers.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetUsers")
        .WithSummary("Get paginated users");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetUserById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetUserById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetUserById")
        .WithSummary("Get user by id");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateUser.Request request,
            [FromServices] UpdateUser.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateUser.Command(
                id, request.FirstName, request.LastName, request.PhoneNumber,
                request.AvatarUrl, request.IsActive, request.PreferredLanguage, request.TimeZone);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateUser")
        .WithSummary("Update user profile");

        group.MapPost("/{id:guid}/toggle-active", async (
            Guid id,
            [FromServices] ToggleUserActive.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new ToggleUserActive.Command(id), ct);
            return result.ToHttpResult();
        })
        .WithName("ToggleUserActive")
        .WithSummary("Toggle user active status");

        group.MapPut("/{id:guid}/roles", async (
            Guid id,
            [FromBody] AssignRolesToUserRequest request,
            [FromServices] AssignRolesToUser.Handler handler,
            CancellationToken ct) =>
        {
            var command = new AssignRolesToUser.Command(id, request.RoleIds);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("AssignRolesToUser")
        .WithSummary("Assign roles to a user");

        group.MapGet("/{id:guid}/menus", async (
            Guid id,
            [FromServices] GetUserMenus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetUserMenus.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetUserMenus")
        .WithSummary("Get user accessible menus");

        group.MapGet("/{id:guid}/permissions", async (
            Guid id,
            [FromServices] GetUserPermissions.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetUserPermissions.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetUserPermissions")
        .WithSummary("Get user permission codes");
    }
}

record AssignRolesToUserRequest(List<Guid> RoleIds);
