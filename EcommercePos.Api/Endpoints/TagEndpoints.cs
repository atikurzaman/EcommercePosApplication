using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tags").WithTags("Tags");

        group.MapGet("/", async (
            [AsParameters] GetTagsRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Tags
                .Where(t => !t.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(t => t.Name.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(t => t.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TagResponse(
                    t.Id, t.Name, t.Slug, t.CreatedAt))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetTags")
        .WithSummary("Get paginated tags");

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var tag = await context.Tags
                .Where(t => t.Id == id && !t.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (tag == null)
                return Results.NotFound(new { error = "Tag not found" });

            return Results.Ok(new { data = new TagResponse(
                tag.Id, tag.Name, tag.Slug, tag.CreatedAt) });
        })
        .WithName("GetTagById")
        .WithSummary("Get tag by id");

        group.MapPost("/", async (CreateTagRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var tag = new Tags
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-"),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Tags.Add(tag);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/tags/{tag.Id}", new { data = new TagResponse(
                tag.Id, tag.Name, tag.Slug, tag.CreatedAt) });
        })
        .WithName("CreateTag")
        .WithSummary("Create a new tag");

        group.MapPut("/{id:guid}", async (Guid id, UpdateTagRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var tag = await context.Tags.FindAsync(new object[] { id }, ct);
            if (tag == null || tag.IsDeleted)
                return Results.NotFound(new { error = "Tag not found" });

            tag.Name = request.Name;
            tag.Slug = request.Slug ?? request.Name.ToLower().Replace(" ", "-");
            tag.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new TagResponse(
                tag.Id, tag.Name, tag.Slug, tag.CreatedAt) });
        })
        .WithName("UpdateTag")
        .WithSummary("Update an existing tag");

        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var tag = await context.Tags.FindAsync(new object[] { id }, ct);
            if (tag == null || tag.IsDeleted)
                return Results.NotFound(new { error = "Tag not found" });

            tag.IsDeleted = true;
            tag.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteTag")
        .WithSummary("Soft delete a tag");
    }
}

public record GetTagsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record TagResponse(
    Guid Id, string Name, string Slug, DateTime CreatedAt);
public record CreateTagRequest(
    string Name, string? Slug = null);
public record UpdateTagRequest(
    string Name, string? Slug = null);