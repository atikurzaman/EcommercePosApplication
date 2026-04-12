using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Security;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class MenuEndpoints
{
    public static void MapMenuEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/menus").WithTags("Menus");

        group.MapGet("/", async (
            [AsParameters] GetMenus.Request request,
            [FromServices] GetMenus.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetMenus")
        .WithSummary("Get paginated menus");

        group.MapGet("/tree", async (
            [FromServices] GetMenuTree.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(ct);
            return result.ToHttpResult();
        })
        .WithName("GetMenuTree")
        .WithSummary("Get full menu tree structure");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetMenuById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetMenuById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetMenuById")
        .WithSummary("Get menu by id");

        group.MapPost("/", async (
            [FromBody] CreateMenu.Request request,
            [FromServices] CreateMenu.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/menus");
        })
        .WithName("CreateMenu")
        .WithSummary("Create a new menu");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateMenu.Request request,
            [FromServices] UpdateMenu.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateMenu.Command(
                id, request.MenuCode, request.MenuName, request.DisplayName, request.MenuUrl,
                request.IconClass, request.DisplayOrder, request.MenuLevel, request.PermissionCode,
                request.ParentMenuId, request.IsActive, request.IsVisible, request.IsExternalLink,
                request.OpenInNewTab, request.Description);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateMenu")
        .WithSummary("Update an existing menu");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteMenu.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteMenu.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteMenu")
        .WithSummary("Soft delete a menu");
    }
}
