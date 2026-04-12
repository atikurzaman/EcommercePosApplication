using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Catalog;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class SpecificationEndpoints
{
    public static void MapSpecificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/specifications").WithTags("Specifications");

        group.MapGet("/", async (
            [AsParameters] GetSpecifications.Request request,
            [FromServices] GetSpecifications.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetSpecifications.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToPagedResult();
        })
        .WithName("GetSpecifications")
        .WithSummary("Get paginated specifications");

        group.MapPost("/", async (
            [FromBody] CreateSpecification.Request body,
            [FromServices] CreateSpecification.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateSpecification.Command(body.SpecName, body.SortOrder);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult("/api/specifications");
        })
        .WithName("CreateSpecification")
        .WithSummary("Create a new specification");
    }
}
