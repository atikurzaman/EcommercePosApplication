using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ReturnStatusEndpoints
{
    public static void MapReturnStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/return-statuses").WithTags("ReturnStatuses");

        group.MapGet("/", async (
            [AsParameters] GetReturnStatusesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.ReturnStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.StatusCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ReturnStatusResponse(c.StatusCode, c.DisplayName, c.SortOrder))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetReturnStatuses")
        .WithSummary("Get paginated return statuses");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.ReturnStatuses
                .Where(c => c.StatusCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (status == null)
                return Results.NotFound(new { error = "Return status not found" });

            return Results.Ok(new { data = new ReturnStatusResponse(status.StatusCode, status.DisplayName, status.SortOrder) });
        })
        .WithName("GetReturnStatusByCode")
        .WithSummary("Get return status by code");

        group.MapPost("/", async (CreateReturnStatusRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.ReturnStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Return status code already exists" });

            var status = new ReturnStatuses
            {
                StatusCode = request.StatusCode,
                DisplayName = request.DisplayName,
                SortOrder = request.SortOrder
            };

            context.ReturnStatuses.Add(status);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/return-statuses/{status.StatusCode}", new { data = new ReturnStatusResponse(
                status.StatusCode, status.DisplayName, status.SortOrder) });
        })
        .WithName("CreateReturnStatus")
        .WithSummary("Create a new return status");

        group.MapPut("/{code}", async (string code, UpdateReturnStatusRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.ReturnStatuses.FirstOrDefaultAsync(c => c.StatusCode == code, ct);
            if (status == null)
                return Results.NotFound(new { error = "Return status not found" });

            if (status.StatusCode != request.StatusCode)
            {
                var exists = await context.ReturnStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Return status code already exists" });
            }

            status.StatusCode = request.StatusCode;
            status.DisplayName = request.DisplayName;
            status.SortOrder = request.SortOrder;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new ReturnStatusResponse(
                status.StatusCode, status.DisplayName, status.SortOrder) });
        })
        .WithName("UpdateReturnStatus")
        .WithSummary("Update an existing return status");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.ReturnStatuses.FirstOrDefaultAsync(c => c.StatusCode == code, ct);
            if (status == null)
                return Results.NotFound(new { error = "Return status not found" });

            context.ReturnStatuses.Remove(status);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteReturnStatus")
        .WithSummary("Delete a return status");
    }
}

public record GetReturnStatusesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record ReturnStatusResponse(string StatusCode, string DisplayName, byte SortOrder);
public record CreateReturnStatusRequest(string StatusCode, string DisplayName, byte SortOrder);
public record UpdateReturnStatusRequest(string StatusCode, string DisplayName, byte SortOrder);
