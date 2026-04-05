using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class CustomerTierEndpoints
{
    public static void MapCustomerTierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customer-tiers").WithTags("CustomerTiers");

        group.MapGet("/", async (
            [AsParameters] GetCustomerTiersRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.CustomerTiers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.TierCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CustomerTierResponse(c.TierCode, c.DisplayName, c.MinLifetimeSpend, c.DiscountPct, c.PointsMultiplier, c.SortOrder))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetCustomerTiers")
        .WithSummary("Get paginated customer tiers");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var tier = await context.CustomerTiers
                .Where(c => c.TierCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (tier == null)
                return Results.NotFound(new { error = "Customer tier not found" });

            return Results.Ok(new { data = new CustomerTierResponse(
                tier.TierCode, tier.DisplayName, tier.MinLifetimeSpend, tier.DiscountPct, tier.PointsMultiplier, tier.SortOrder) });
        })
        .WithName("GetCustomerTierByCode")
        .WithSummary("Get customer tier by code");

        group.MapPost("/", async (CreateCustomerTierRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.CustomerTiers.AnyAsync(c => c.TierCode == request.TierCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Customer tier code already exists" });

            var tier = new CustomerTiers
            {
                TierCode = request.TierCode,
                DisplayName = request.DisplayName,
                MinLifetimeSpend = request.MinLifetimeSpend,
                DiscountPct = request.DiscountPct,
                PointsMultiplier = request.PointsMultiplier,
                SortOrder = request.SortOrder
            };

            context.CustomerTiers.Add(tier);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/customer-tiers/{tier.TierCode}", new { data = new CustomerTierResponse(
                tier.TierCode, tier.DisplayName, tier.MinLifetimeSpend, tier.DiscountPct, tier.PointsMultiplier, tier.SortOrder) });
        })
        .WithName("CreateCustomerTier")
        .WithSummary("Create a new customer tier");

        group.MapPut("/{code}", async (string code, UpdateCustomerTierRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var tier = await context.CustomerTiers.FirstOrDefaultAsync(c => c.TierCode == code, ct);
            if (tier == null)
                return Results.NotFound(new { error = "Customer tier not found" });

            if (tier.TierCode != request.TierCode)
            {
                var exists = await context.CustomerTiers.AnyAsync(c => c.TierCode == request.TierCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Customer tier code already exists" });
            }

            tier.TierCode = request.TierCode;
            tier.DisplayName = request.DisplayName;
            tier.MinLifetimeSpend = request.MinLifetimeSpend;
            tier.DiscountPct = request.DiscountPct;
            tier.PointsMultiplier = request.PointsMultiplier;
            tier.SortOrder = request.SortOrder;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new CustomerTierResponse(
                tier.TierCode, tier.DisplayName, tier.MinLifetimeSpend, tier.DiscountPct, tier.PointsMultiplier, tier.SortOrder) });
        })
        .WithName("UpdateCustomerTier")
        .WithSummary("Update an existing customer tier");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var tier = await context.CustomerTiers.FirstOrDefaultAsync(c => c.TierCode == code, ct);
            if (tier == null)
                return Results.NotFound(new { error = "Customer tier not found" });

            context.CustomerTiers.Remove(tier);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteCustomerTier")
        .WithSummary("Delete a customer tier");
    }
}

public record GetCustomerTiersRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record CustomerTierResponse(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);
public record CreateCustomerTierRequest(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);
public record UpdateCustomerTierRequest(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);
