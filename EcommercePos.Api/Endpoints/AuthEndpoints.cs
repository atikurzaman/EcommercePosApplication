using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Auth;
using EcommercePos.Shared.Common;

namespace EcommercePos.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", async (
            [FromBody] RegisterUser.Request request,
            [FromServices] RegisterUser.Handler handler,
            CancellationToken ct) =>
        {
            var command = new RegisterUser.Command(request.Email, request.Password, request.FirstName, request.LastName, request.Phone);
            var result = await handler.Handle(command, ct);
            if (!result.IsSuccess) return Results.BadRequest(new { message = result.Error?.Message });
            return Results.Created($"/api/auth/register", result.Value);
        })
        .AllowAnonymous()
        .WithName("Register")
        .WithSummary("Register a new user");

        group.MapPost("/login", async (
            [FromBody] LoginUser.Request request,
            [FromServices] LoginUser.Handler handler,
            CancellationToken ct) =>
        {
            var command = new LoginUser.Command(request.Email, request.Password);
            var result = await handler.Handle(command, ct);
            if (!result.IsSuccess) return Results.Json(new { message = result.Error?.Message }, statusCode: 401);
            return Results.Ok(result.Value);
        })
        .AllowAnonymous()
        .WithName("Login")
        .WithSummary("Login user");

        group.MapGet("/me", async (
            [FromServices] GetCurrentUser.Handler handler,
            System.Security.Claims.ClaimsPrincipal claims,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCurrentUser.Query(), claims, ct);
            if (!result.IsSuccess) return Results.Unauthorized();
            return Results.Ok(result.Value);
        })
        .WithName("GetCurrentUser")
        .WithSummary("Get current user");

        group.MapPost("/change-password", async (
            [FromBody] ChangePassword.Request request,
            [FromServices] ChangePassword.Handler handler,
            System.Security.Claims.ClaimsPrincipal claims,
            CancellationToken ct) =>
        {
            var userId = claims.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || !Guid.TryParse(userId, out var id)) return Results.Unauthorized();
            var command = new ChangePassword.Command(id, request.CurrentPassword, request.NewPassword);
            var result = await handler.Handle(command, ct);
            if (!result.IsSuccess) return Results.BadRequest(new { message = result.Error?.Message });
            return Results.Ok(new { message = "Password changed successfully" });
        })
        .WithName("ChangePassword")
        .WithSummary("Change password");

        group.MapGet("/roles", async (
            [FromServices] GetRoles.Handler handler,
            CancellationToken ct) =>
        {
            var roles = await handler.Handle(new GetRoles.Query(), ct);
            return Results.Ok(roles);
        })
        .WithName("GetUserRoles")
        .WithSummary("Get all roles");
    }
}
