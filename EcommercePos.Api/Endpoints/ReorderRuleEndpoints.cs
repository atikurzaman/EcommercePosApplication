using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ReorderRuleEndpoints
{
    public static void MapReorderRuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reorder-rules").WithTags("ReorderRules");

        group.MapGet("/", async (
            [AsParameters] GetReorderRulesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.ReorderRules
                .Include(r => r.Product)
                .Include(r => r.Warehouse)
                .Include(r => r.PreferredSupplier)
                .Where(r => !r.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.WarehouseId))
                query = query.Where(r => r.WarehouseId == Guid.Parse(request.WarehouseId));

            if (request.ActiveOnly == true)
                query = query.Where(r => r.IsActive);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(r => r.Product.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new ReorderRuleResponse(
                    r.Id, r.ProductId, r.Product.Name, r.VariantId,
                    r.WarehouseId, r.Warehouse != null ? r.Warehouse.Name : null,
                    r.PreferredSupplierId, r.PreferredSupplier != null ? r.PreferredSupplier.Name : null,
                    r.ReorderLevel, r.ReorderQuantity, r.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetReorderRules")
        .WithSummary("Get paginated reorder rules");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var rule = await context.ReorderRules
                .Include(r => r.Product)
                .Include(r => r.Warehouse)
                .Include(r => r.PreferredSupplier)
                .Include(r => r.NotifyUser)
                .Where(r => r.Id == id && !r.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (rule == null)
                return Results.NotFound(new { error = "Reorder rule not found" });

            var response = new ReorderRuleDetailResponse(
                rule.Id, rule.ProductId, rule.Product.Name, rule.VariantId,
                rule.WarehouseId, rule.Warehouse != null ? rule.Warehouse.Name : null,
                rule.PreferredSupplierId, rule.PreferredSupplier != null ? rule.PreferredSupplier.Name : null,
                rule.ReorderLevel, rule.ReorderQuantity,
                rule.NotifyUserId, rule.NotifyUser != null ? rule.NotifyUser.FirstName + " " + rule.NotifyUser.LastName : null,
                rule.IsActive, rule.CreatedAt);

            return Results.Ok(new { data = response });
        })
        .WithName("GetReorderRuleById")
        .WithSummary("Get reorder rule details");

        group.MapPost("/", async (CreateReorderRuleRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var rule = new ReorderRules
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                VariantId = request.VariantId,
                WarehouseId = request.WarehouseId,
                PreferredSupplierId = request.PreferredSupplierId,
                ReorderLevel = request.ReorderLevel,
                ReorderQuantity = request.ReorderQuantity,
                NotifyUserId = request.NotifyUserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.ReorderRules.Add(rule);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/reorder-rules/{rule.Id}", new { data = new { rule.Id } });
        })
        .WithName("CreateReorderRule")
        .WithSummary("Create reorder rule");

        group.MapPut("/{id:guid}", async (Guid id, UpdateReorderRuleRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var rule = await context.ReorderRules.FindAsync(new object[] { id }, ct);
            if (rule == null || rule.IsDeleted)
                return Results.NotFound(new { error = "Reorder rule not found" });

            rule.WarehouseId = request.WarehouseId;
            rule.PreferredSupplierId = request.PreferredSupplierId;
            rule.ReorderLevel = request.ReorderLevel;
            rule.ReorderQuantity = request.ReorderQuantity;
            rule.NotifyUserId = request.NotifyUserId;
            rule.IsActive = request.IsActive;
            rule.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);

            return Results.Ok(new { data = new { rule.Id } });
        })
        .WithName("UpdateReorderRule")
        .WithSummary("Update reorder rule");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var rule = await context.ReorderRules.FindAsync(new object[] { id }, ct);
            if (rule == null || rule.IsDeleted)
                return Results.NotFound(new { error = "Reorder rule not found" });

            rule.IsDeleted = true;
            rule.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);

            return Results.NoContent();
        })
        .WithName("DeleteReorderRule")
        .WithSummary("Delete reorder rule");

        group.MapPost("/{id:guid}/toggle-active", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var rule = await context.ReorderRules.FindAsync(new object[] { id }, ct);
            if (rule == null || rule.IsDeleted)
                return Results.NotFound(new { error = "Reorder rule not found" });

            rule.IsActive = !rule.IsActive;
            rule.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);

            return Results.Ok(new { data = new { rule.Id, rule.IsActive } });
        })
        .WithName("ToggleReorderRuleActive")
        .WithSummary("Toggle reorder rule active status");
    }
}

public record GetReorderRulesRequest(
    int PageIndex = 0, int PageSize = 10, string? WarehouseId = null, bool? ActiveOnly = null);

public record ReorderRuleResponse(
    Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
    Guid? WarehouseId, string? WarehouseName,
    Guid? PreferredSupplierId, string? PreferredSupplierName,
    decimal ReorderLevel, decimal ReorderQuantity, bool IsActive);

public record ReorderRuleDetailResponse(
    Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
    Guid? WarehouseId, string? WarehouseName,
    Guid? PreferredSupplierId, string? PreferredSupplierName,
    decimal ReorderLevel, decimal ReorderQuantity,
    Guid? NotifyUserId, string? NotifyUserName,
    bool IsActive, DateTime CreatedAt);

public record CreateReorderRuleRequest(
    Guid ProductId, Guid? VariantId, Guid? WarehouseId,
    Guid? PreferredSupplierId, decimal ReorderLevel, decimal ReorderQuantity,
    Guid? NotifyUserId);

public record UpdateReorderRuleRequest(
    Guid? WarehouseId, Guid? PreferredSupplierId,
    decimal ReorderLevel, decimal ReorderQuantity,
    Guid? NotifyUserId, bool IsActive);