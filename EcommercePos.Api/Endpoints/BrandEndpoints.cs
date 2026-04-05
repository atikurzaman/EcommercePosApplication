using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Brand.Queries;
using EcommercePos.Application.Features.Brand.Commands;
using EcommercePos.Api.Extensions;

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
    }
}

public record GetBrandsRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record CreateBrandRequest(string Name, string? Description, string? LogoUrl, string? Website, string? CountryOfOrigin, bool IsFeatured, bool IsActive, string? BrandCode = null);
public record UpdateBrandRequest(string Name, string? Description, string? LogoUrl, string? Website, string? CountryOfOrigin, bool IsFeatured, bool IsActive, string? BrandCode = null);
