using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.ReorderRule;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class ReorderRuleEndpoints
{
    public static void MapReorderRuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reorder-rules").WithTags("ReorderRules");

        group.MapGet("/", async (
            [AsParameters] GetReorderRules.Query request,
            GetReorderRules.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToPagedResult())
        .WithName("GetReorderRules")
        .WithSummary("Get paginated reorder rules");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetReorderRuleById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetReorderRuleById.Query(id), ct)).ToHttpResult())
        .WithName("GetReorderRuleById")
        .WithSummary("Get reorder rule details");

        group.MapPost("/", async (
            [FromBody] CreateReorderRule.Command command,
            CreateReorderRule.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/reorder-rules"))
        .AddEndpointFilter<ValidationFilter<CreateReorderRule.Command>>()
        .WithName("CreateReorderRule")
        .WithSummary("Create reorder rule");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateReorderRule.Command command,
            UpdateReorderRule.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateReorderRule.Command(
                id,
                command.WarehouseId,
                command.PreferredSupplierId,
                command.ReorderLevel,
                command.ReorderQuantity,
                command.NotifyUserId,
                command.IsActive), ct)).ToHttpResult())
        .WithName("UpdateReorderRule")
        .WithSummary("Update reorder rule");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteReorderRule.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteReorderRule.Command(id), ct)).ToNoContentResult())
        .WithName("DeleteReorderRule")
        .WithSummary("Delete reorder rule");

        group.MapPost("/{id:guid}/toggle-active", async (
            Guid id,
            ToggleReorderRuleActive.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ToggleReorderRuleActive.Command(id), ct)).ToHttpResult())
        .WithName("ToggleReorderRuleActive")
        .WithSummary("Toggle reorder rule active status");
    }
}