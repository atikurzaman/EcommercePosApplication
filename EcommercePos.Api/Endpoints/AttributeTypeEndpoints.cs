using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class AttributeTypeEndpoints
{
    public static void MapAttributeTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attribute-types").WithTags("AttributeTypes");

        group.MapGet("/", async (
            [AsParameters] GetAttributeTypesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.AttributeTypes
                .Where(a => !a.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(a => a.Name.Contains(request.Search) || a.Slug.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(a => a.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new AttributeTypeResponse(
                    a.Id, a.Name, a.Slug, a.UiType, a.AffectsPrice,
                    a.AffectsSku, a.AffectsImage, a.AffectsStock, a.IsFilterable, a.SortOrder))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetAttributeTypes")
        .WithSummary("Get paginated attribute types");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var type = await context.AttributeTypes
                .Where(a => a.Id == id && !a.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (type == null)
                return Results.NotFound(new { error = "Attribute type not found" });

            return Results.Ok(new { data = new AttributeTypeResponse(
                type.Id, type.Name, type.Slug, type.UiType, type.AffectsPrice,
                type.AffectsSku, type.AffectsImage, type.AffectsStock, type.IsFilterable, type.SortOrder) });
        })
        .WithName("GetAttributeTypeById")
        .WithSummary("Get attribute type by id");

        group.MapPost("/", async (CreateAttributeTypeRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var type = new AttributeTypes
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-"),
                UiType = request.UiType,
                AffectsPrice = request.AffectsPrice,
                AffectsSku = request.AffectsSku,
                AffectsImage = request.AffectsImage,
                AffectsStock = request.AffectsStock,
                IsFilterable = request.IsFilterable,
                SortOrder = request.SortOrder,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.AttributeTypes.Add(type);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/attribute-types/{type.Id}", new { data = new AttributeTypeResponse(
                type.Id, type.Name, type.Slug, type.UiType, type.AffectsPrice,
                type.AffectsSku, type.AffectsImage, type.AffectsStock, type.IsFilterable, type.SortOrder) });
        })
        .WithName("CreateAttributeType")
        .WithSummary("Create a new attribute type");

        group.MapPut("/{id:guid}", async (Guid id, UpdateAttributeTypeRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var type = await context.AttributeTypes.FindAsync(new object[] { id }, ct);
            if (type == null || type.IsDeleted)
                return Results.NotFound(new { error = "Attribute type not found" });

            type.Name = request.Name;
            type.Slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-");
            type.UiType = request.UiType;
            type.AffectsPrice = request.AffectsPrice;
            type.AffectsSku = request.AffectsSku;
            type.AffectsImage = request.AffectsImage;
            type.AffectsStock = request.AffectsStock;
            type.IsFilterable = request.IsFilterable;
            type.SortOrder = request.SortOrder;
            type.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new AttributeTypeResponse(
                type.Id, type.Name, type.Slug, type.UiType, type.AffectsPrice,
                type.AffectsSku, type.AffectsImage, type.AffectsStock, type.IsFilterable, type.SortOrder) });
        })
        .WithName("UpdateAttributeType")
        .WithSummary("Update an existing attribute type");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var type = await context.AttributeTypes.FindAsync(new object[] { id }, ct);
            if (type == null || type.IsDeleted)
                return Results.NotFound(new { error = "Attribute type not found" });

            type.IsDeleted = true;
            type.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteAttributeType")
        .WithSummary("Soft delete an attribute type");
    }
}

public record GetAttributeTypesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record AttributeTypeResponse(
    Guid Id, string Name, string Slug, string UiType, bool AffectsPrice,
    bool AffectsSku, bool AffectsImage, bool AffectsStock, bool IsFilterable, int SortOrder);
public record CreateAttributeTypeRequest(
    string Name, string UiType, bool AffectsPrice, bool AffectsSku,
    bool AffectsImage, bool AffectsStock, bool IsFilterable, int SortOrder, string? Slug = null);
public record UpdateAttributeTypeRequest(
    string Name, string UiType, bool AffectsPrice, bool AffectsSku,
    bool AffectsImage, bool AffectsStock, bool IsFilterable, int SortOrder, string? Slug = null);
