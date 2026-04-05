using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class PaymentMethodEndpoints
{
    public static void MapPaymentMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payment-methods").WithTags("PaymentMethods");

        group.MapGet("/", async (
            [AsParameters] GetPaymentMethodsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.PaymentMethods.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.MethodCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new PaymentMethodResponse(
                    c.MethodCode, c.DisplayName, c.IsOnline, c.IsActive, c.SortOrder))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetPaymentMethods")
        .WithSummary("Get paginated payment methods");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var method = await context.PaymentMethods
                .Where(c => c.MethodCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (method == null)
                return Results.NotFound(new { error = "Payment method not found" });

            return Results.Ok(new { data = new PaymentMethodResponse(
                method.MethodCode, method.DisplayName, method.IsOnline, method.IsActive, method.SortOrder) });
        })
        .WithName("GetPaymentMethodByCode")
        .WithSummary("Get payment method by code");

        group.MapPost("/", async (CreatePaymentMethodRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.PaymentMethods.AnyAsync(c => c.MethodCode == request.MethodCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Payment method code already exists" });

            var method = new PaymentMethods
            {
                MethodCode = request.MethodCode,
                DisplayName = request.DisplayName,
                IsOnline = request.IsOnline,
                IsActive = request.IsActive,
                SortOrder = request.SortOrder
            };

            context.PaymentMethods.Add(method);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/payment-methods/{method.MethodCode}", new { data = new PaymentMethodResponse(
                method.MethodCode, method.DisplayName, method.IsOnline, method.IsActive, method.SortOrder) });
        })
        .WithName("CreatePaymentMethod")
        .WithSummary("Create a new payment method");

        group.MapPut("/{code}", async (string code, UpdatePaymentMethodRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var method = await context.PaymentMethods.FirstOrDefaultAsync(c => c.MethodCode == code, ct);
            if (method == null)
                return Results.NotFound(new { error = "Payment method not found" });

            if (method.MethodCode != request.MethodCode)
            {
                var exists = await context.PaymentMethods.AnyAsync(c => c.MethodCode == request.MethodCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Payment method code already exists" });
            }

            method.MethodCode = request.MethodCode;
            method.DisplayName = request.DisplayName;
            method.IsOnline = request.IsOnline;
            method.IsActive = request.IsActive;
            method.SortOrder = request.SortOrder;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new PaymentMethodResponse(
                method.MethodCode, method.DisplayName, method.IsOnline, method.IsActive, method.SortOrder) });
        })
        .WithName("UpdatePaymentMethod")
        .WithSummary("Update an existing payment method");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var method = await context.PaymentMethods.FirstOrDefaultAsync(c => c.MethodCode == code, ct);
            if (method == null)
                return Results.NotFound(new { error = "Payment method not found" });

            context.PaymentMethods.Remove(method);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeletePaymentMethod")
        .WithSummary("Delete a payment method");
    }
}

public record GetPaymentMethodsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record PaymentMethodResponse(
    string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);
public record CreatePaymentMethodRequest(
    string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);
public record UpdatePaymentMethodRequest(
    string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);
