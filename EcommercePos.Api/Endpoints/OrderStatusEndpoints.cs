using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class OrderStatusEndpoints
{
    public static void MapOrderStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/order-statuses").WithTags("OrderStatuses");

        group.MapGet("/", async (
            [AsParameters] GetOrderStatusesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.OrderStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.StatusCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new OrderStatusResponse(c.StatusCode, c.DisplayName, c.Description, c.SortOrder, c.IsTerminal))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetOrderStatuses")
        .WithSummary("Get paginated order statuses");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.OrderStatuses
                .Where(c => c.StatusCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (status == null)
                return Results.NotFound(new { error = "Order status not found" });

            return Results.Ok(new { data = new OrderStatusResponse(
                status.StatusCode, status.DisplayName, status.Description, status.SortOrder, status.IsTerminal) });
        })
        .WithName("GetOrderStatusByCode")
        .WithSummary("Get order status by code");

        group.MapPost("/", async (CreateOrderStatusRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.OrderStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Order status code already exists" });

            var status = new OrderStatuses
            {
                StatusCode = request.StatusCode,
                DisplayName = request.DisplayName,
                Description = request.Description,
                SortOrder = request.SortOrder,
                IsTerminal = request.IsTerminal
            };

            context.OrderStatuses.Add(status);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/order-statuses/{status.StatusCode}", new { data = new OrderStatusResponse(
                status.StatusCode, status.DisplayName, status.Description, status.SortOrder, status.IsTerminal) });
        })
        .WithName("CreateOrderStatus")
        .WithSummary("Create a new order status");

        group.MapPut("/{code}", async (string code, UpdateOrderStatusRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.OrderStatuses.FirstOrDefaultAsync(c => c.StatusCode == code, ct);
            if (status == null)
                return Results.NotFound(new { error = "Order status not found" });

            if (status.StatusCode != request.StatusCode)
            {
                var exists = await context.OrderStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Order status code already exists" });
            }

            status.StatusCode = request.StatusCode;
            status.DisplayName = request.DisplayName;
            status.Description = request.Description;
            status.SortOrder = request.SortOrder;
            status.IsTerminal = request.IsTerminal;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new OrderStatusResponse(
                status.StatusCode, status.DisplayName, status.Description, status.SortOrder, status.IsTerminal) });
        })
        .WithName("UpdateOrderStatus")
        .WithSummary("Update an existing order status");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.OrderStatuses.FirstOrDefaultAsync(c => c.StatusCode == code, ct);
            if (status == null)
                return Results.NotFound(new { error = "Order status not found" });

            context.OrderStatuses.Remove(status);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteOrderStatus")
        .WithSummary("Delete an order status");
    }
}

public record GetOrderStatusesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record OrderStatusResponse(string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);
public record CreateOrderStatusRequest(string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);
public record UpdateOrderStatusRequest(string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);
