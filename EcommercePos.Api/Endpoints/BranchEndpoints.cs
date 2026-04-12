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
            [FromBody] UpdateBranchBody body,
            UpdateBranch.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateBranch.Command(
                id, body.WarehouseCode, body.Name, body.Description,
                body.AddressLine1, body.AddressLine2, body.City,
                body.Area, body.State, body.PostalCode,
                body.Phone, body.Email, body.IsActive), ct)).ToHttpResult())
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

public record UpdateBranchBody(
    string WarehouseCode, string Name, string? Description,
    string? AddressLine1, string? AddressLine2, string? City,
    string? Area, string? State, string? PostalCode,
    string? Phone, string? Email, bool IsActive);
