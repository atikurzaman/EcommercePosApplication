using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Brand;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

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

        group.MapGet("/with-count", async (
            GetBrandsWithCount.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetBrandsWithCount.Query(), ct)).ToHttpResult())
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
            .AddEndpointFilter<ValidationFilter<CreateBrand.Command>>()
            .WithName("CreateBrand")
            .WithSummary("Create a new brand");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateBrand.Command body,
            UpdateBrand.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(body with { Id = id }, ct)).ToHttpResult())
            .AddEndpointFilter<ValidationFilter<UpdateBrand.Command>>()
            .WithName("UpdateBrand")
            .WithSummary("Update an existing brand");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteBrand.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteBrand.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteBrand")
            .WithSummary("Soft delete a brand");

        group.MapPatch("/{id:guid}/toggle", async (
            Guid id,
            ToggleBrandActive.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new ToggleBrandActive.Command(id), ct)).ToHttpResult())
            .WithName("ToggleBrand")
            .WithSummary("Toggle brand active status");
    }
}
