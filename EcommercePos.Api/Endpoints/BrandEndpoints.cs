using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Brand;
using EcommercePos.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class BrandEndpoints
{
    public static void MapBrandEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/brands").WithTags("Brands");

        group.MapGet("/", async (
            [AsParameters] GetBrands.Query query,
            GetBrands.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetBrands")
            .WithSummary("Get paginated brands");

        group.MapGet("/with-count", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var brands = await context.Brands
                .Where(b => !b.IsDeleted)
                .OrderBy(b => b.Name)
                .Select(b => new
                {
                    b.Id, b.BrandCode, b.Name, b.Slug, b.Description,
                    b.LogoUrl, b.Website, b.IsFeatured, b.IsActive,
                    ProductCount = context.Products.Count(p => p.BrandId == b.Id && !p.IsDeleted)
                })
                .ToListAsync(ct);

            return Results.Ok(new { data = brands });
        })
        .WithName("GetBrandsWithCount")
        .WithSummary("Get brands with product count");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetBrandById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetBrandById.Query(id), ct)).ToHttpResult())
            .WithName("GetBrandById")
            .WithSummary("Get brand by id");

        group.MapPost("/", async (
            [FromBody] CreateBrand.Command command,
            CreateBrand.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/brands"))
            .WithName("CreateBrand")
            .WithSummary("Create a new brand");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateBrandBody body,
            UpdateBrand.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateBrand.Command(
                id, body.Name, body.BrandCode, body.Description, body.LogoUrl,
                body.Website, body.CountryOfOrigin, body.IsFeatured, body.IsActive), ct)).ToHttpResult())
            .WithName("UpdateBrand")
            .WithSummary("Update an existing brand");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteBrand.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteBrand.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteBrand")
            .WithSummary("Soft delete a brand");

        group.MapPatch("/{id:guid}/toggle", async (Guid id, ApplicationDbContext context, CancellationToken ct) =>
        {
            var brand = await context.Brands.FindAsync(new object[] { id }, ct);
            if (brand == null || brand.IsDeleted)
                return Results.NotFound(new { error = "Brand not found" });

            brand.IsActive = !brand.IsActive;
            brand.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            return Results.Ok(new { data = new { brand.Id, brand.IsActive } });
        })
        .WithName("ToggleBrand")
        .WithSummary("Toggle brand active status");
    }
}

public record UpdateBrandBody(
    string Name, string? BrandCode, string? Description, string? LogoUrl,
    string? Website, string? CountryOfOrigin, bool IsFeatured, bool IsActive);
