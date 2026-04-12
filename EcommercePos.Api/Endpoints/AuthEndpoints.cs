using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Auth;
using EcommercePos.Api.Extensions;
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
            return result.ToCreatedResult("/api/auth/register");
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
            return result.ToHttpResult();
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
            return result.ToHttpResult();
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
            if (userId == null || !Guid.TryParse(userId, out var id))
                return Result.Failure(Error.Unauthorized("Invalid user context")).ToHttpResult();

            var command = new ChangePassword.Command(id, request.CurrentPassword, request.NewPassword);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ChangePassword")
        .WithSummary("Change password");

        group.MapGet("/roles", async (
            [FromServices] GetRoles.Handler handler,
            CancellationToken ct) =>
        {
            var roles = await handler.Handle(new GetRoles.Query(), ct);
            return Result<List<GetRoles.Response>>.Success(roles).ToHttpResult();
        })
        .WithName("GetUserRoles")
        .WithSummary("Get all roles");
    }
}
