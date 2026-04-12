using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Brand.Queries;
using EcommercePos.Application.Features.Brand.Commands;
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
            [AsParameters] GetBrandsRequest request,
            [FromServices] GetBrands.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetBrands.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetBrands")
        .WithSummary("Get paginated brands");

        group.MapGet("/with-count", async (ApplicationDbContext context, CancellationToken ct) =>
        {
            var brands = await context.Brands
                .Where(b => !b.IsDeleted)
                .OrderBy(b => b.Name)
                .Select(b => new
                {
                    b.Id,
                    b.BrandCode,
                    b.Name,
                    b.Slug,
                    b.Description,
                    b.LogoUrl,
                    b.Website,
                    b.IsFeatured,
                    b.IsActive,
                    ProductCount = context.Products.Count(p => p.BrandId == b.Id && !p.IsDeleted)
                })
                .ToListAsync(ct);

            return Results.Ok(new { data = brands });
        })
        .WithName("GetBrandsWithCount")
        .WithSummary("Get brands with product count");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetBrandById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetBrandById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetBrandById")
        .WithSummary("Get brand by id");

        group.MapPost("/", async (
            [FromBody] CreateBrandRequest request,
            [FromServices] CreateBrand.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateBrand.Command(
                request.BrandCode ?? request.Name.ToUpper().Replace(" ", "").Substring(0, Math.Min(10, request.Name.Length)),
                request.Name, request.Description, request.LogoUrl,
                request.Website, request.CountryOfOrigin, request.IsFeatured, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/brands/{command.Name}");
        })
        .WithName("CreateBrand")
        .WithSummary("Create a new brand");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateBrandRequest request,
            [FromServices] UpdateBrand.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateBrand.Command(
                id, request.BrandCode ?? request.Name.ToUpper().Replace(" ", "").Substring(0, Math.Min(10, request.Name.Length)),
                request.Name, request.Description, request.LogoUrl,
                request.Website, request.CountryOfOrigin, request.IsFeatured, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateBrand")
        .WithSummary("Update an existing brand");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteBrand.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteBrand.Command(id), ct);
            return result.ToNoContentResult();
        })
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

public record GetBrandsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record CreateBrandRequest(string Name, string? Description, string? LogoUrl, string? Website, string? CountryOfOrigin, bool IsFeatured, bool IsActive, string? BrandCode = null);
public record UpdateBrandRequest(string Name, string? Description, string? LogoUrl, string? Website, string? CountryOfOrigin, bool IsFeatured, bool IsActive, string? BrandCode = null);
