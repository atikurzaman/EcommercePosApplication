using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class PaymentStatusEndpoints
{
    public static void MapPaymentStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-statuses").WithTags("PaymentStatuses");

        group.MapGet("/", async (
            [AsParameters] GetPaymentStatusesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.PaymentStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.StatusCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.StatusCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new PaymentStatusResponse(c.StatusCode, c.DisplayName))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetPaymentStatuses")
        .WithSummary("Get paginated payment statuses");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.PaymentStatuses
                .Where(c => c.StatusCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (status == null)
                return Results.NotFound(new { error = "Payment status not found" });

            return Results.Ok(new { data = new PaymentStatusResponse(status.StatusCode, status.DisplayName) });
        })
        .WithName("GetPaymentStatusByCode")
        .WithSummary("Get payment status by code");

        group.MapPost("/", async (CreatePaymentStatusRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.PaymentStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Payment status code already exists" });

            var status = new PaymentStatuses
            {
                StatusCode = request.StatusCode,
                DisplayName = request.DisplayName
            };

            context.PaymentStatuses.Add(status);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/payment-statuses/{status.StatusCode}", new { data = new PaymentStatusResponse(
                status.StatusCode, status.DisplayName) });
        })
        .WithName("CreatePaymentStatus")
        .WithSummary("Create a new payment status");

        group.MapPut("/{code}", async (string code, UpdatePaymentStatusRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.PaymentStatuses.FirstOrDefaultAsync(c => c.StatusCode == code, ct);
            if (status == null)
                return Results.NotFound(new { error = "Payment status not found" });

            if (status.StatusCode != request.StatusCode)
            {
                var exists = await context.PaymentStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Payment status code already exists" });
            }

            status.StatusCode = request.StatusCode;
            status.DisplayName = request.DisplayName;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new PaymentStatusResponse(
                status.StatusCode, status.DisplayName) });
        })
        .WithName("UpdatePaymentStatus")
        .WithSummary("Update an existing payment status");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var status = await context.PaymentStatuses.FirstOrDefaultAsync(c => c.StatusCode == code, ct);
            if (status == null)
                return Results.NotFound(new { error = "Payment status not found" });

            context.PaymentStatuses.Remove(status);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeletePaymentStatus")
        .WithSummary("Delete a payment status");
    }
}

public record GetPaymentStatusesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record PaymentStatusResponse(string StatusCode, string DisplayName);
public record CreatePaymentStatusRequest(string StatusCode, string DisplayName);
public record UpdatePaymentStatusRequest(string StatusCode, string DisplayName);
