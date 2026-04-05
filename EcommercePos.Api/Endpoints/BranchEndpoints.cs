using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Branch.Queries;
using EcommercePos.Application.Features.Branch.Commands;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class BranchEndpoints
{
    public static void MapBranchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/branches").WithTags("Branches");

        group.MapGet("/", async (
            [AsParameters] GetBranchesRequest request, 
            [FromServices] GetBranches.Handler handler, 
            CancellationToken ct) =>
        {
            var query = new GetBranches.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetBranches")
        .WithSummary("Get paginated branches");

        group.MapGet("/{id:guid}", async (
            Guid id, 
            [FromServices] GetBranchById.Handler handler, 
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetBranchById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetBranchById")
        .WithSummary("Get branch by id");

        group.MapPost("/", async (
            [FromBody] CreateBranchRequest request, 
            [FromServices] CreateBranch.Handler handler, 
            CancellationToken ct) =>
        {
            var command = new CreateBranch.Command(
                request.WarehouseCode, request.Name, request.Description,
                request.AddressLine1, request.AddressLine2, request.City,
                request.Area, request.State, request.PostalCode,
                request.Phone, request.Email, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("CreateBranch")
        .WithSummary("Create a new branch");

        group.MapPut("/{id:guid}", async (
            Guid id, 
            [FromBody] UpdateBranchRequest request, 
            [FromServices] UpdateBranch.Handler handler, 
            CancellationToken ct) =>
        {
            var command = new UpdateBranch.Command(
                id, request.WarehouseCode, request.Name, request.Description,
                request.AddressLine1, request.AddressLine2, request.City,
                request.Area, request.State, request.PostalCode,
                request.Phone, request.Email, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateBranch")
        .WithSummary("Update an existing branch");

        group.MapDelete("/{id:guid}", async (
            Guid id, 
            [FromServices] DeleteBranch.Handler handler, 
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteBranch.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteBranch")
        .WithSummary("Soft delete a branch");
    }
}

public record GetBranchesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record CreateBranchRequest(
    string WarehouseCode, string Name, string? Description,
    string? AddressLine1, string? AddressLine2, string? City,
    string? Area, string? State, string? PostalCode,
    string? Phone, string? Email, bool IsActive);
public record UpdateBranchRequest(
    string WarehouseCode, string Name, string? Description,
    string? AddressLine1, string? AddressLine2, string? City,
    string? Area, string? State, string? PostalCode,
    string? Phone, string? Email, bool IsActive);