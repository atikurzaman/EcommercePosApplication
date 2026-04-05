using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class ColorEndpoints
{
    public static void MapColorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/colors").WithTags("Colors");

        group.MapGet("/", async (
            [AsParameters] GetColorsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Colors
                .Where(c => !c.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.Name.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ColorResponse(
                    c.Id, c.Name, c.HexCode, c.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetColors")
        .WithSummary("Get paginated colors");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var color = await context.Colors
                .Where(c => c.Id == id && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (color == null)
                return Results.NotFound(new { error = "Color not found" });

            return Results.Ok(new { data = new ColorResponse(
                color.Id, color.Name, color.HexCode, color.IsActive) });
        })
        .WithName("GetColorById")
        .WithSummary("Get color by id");

        group.MapPost("/", async (CreateColorRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var color = new Colors
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                HexCode = request.HexCode,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Colors.Add(color);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/colors/{color.Id}", new { data = new ColorResponse(
                color.Id, color.Name, color.HexCode, color.IsActive) });
        })
        .WithName("CreateColor")
        .WithSummary("Create a new color");

        group.MapPut("/{id:guid}", async (Guid id, UpdateColorRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var color = await context.Colors.FindAsync(new object[] { id }, ct);
            if (color == null || color.IsDeleted)
                return Results.NotFound(new { error = "Color not found" });

            color.Name = request.Name;
            color.HexCode = request.HexCode;
            color.IsActive = request.IsActive;
            color.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new ColorResponse(
                color.Id, color.Name, color.HexCode, color.IsActive) });
        })
        .WithName("UpdateColor")
        .WithSummary("Update an existing color");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var color = await context.Colors.FindAsync(new object[] { id }, ct);
            if (color == null || color.IsDeleted)
                return Results.NotFound(new { error = "Color not found" });

            color.IsDeleted = true;
            color.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteColor")
        .WithSummary("Soft delete a color");
    }
}

public record GetColorsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record ColorResponse(
    Guid Id, string Name, string? HexCode, bool IsActive);
public record CreateColorRequest(
    string Name, string? HexCode, bool IsActive);
public record UpdateColorRequest(
    string Name, string? HexCode, bool IsActive);
