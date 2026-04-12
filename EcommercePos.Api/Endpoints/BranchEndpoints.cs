using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Branch;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class BranchEndpoints
{
    public static void MapBranchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/branches").WithTags("Branches");

        group.MapGet("/", async (
            [AsParameters] GetBranches.Query query,
            GetBranches.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetBranches")
            .WithSummary("Get paginated branches");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetBranchById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetBranchById.Query(id), ct)).ToHttpResult())
            .WithName("GetBranchById")
            .WithSummary("Get branch by id");

        group.MapPost("/", async (
            [FromBody] CreateBranch.Command command,
            CreateBranch.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/branches"))
            .WithName("CreateBranch")
            .WithSummary("Create a new branch");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateBranch.Command body,
            UpdateBranch.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { Id = id }, ct)).ToHttpResult())
            .WithName("UpdateBranch")
            .WithSummary("Update an existing branch");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteBranch.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteBranch.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteBranch")
            .WithSummary("Soft delete a branch");
    }
}
