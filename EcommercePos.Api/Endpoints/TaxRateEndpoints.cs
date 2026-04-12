using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.TaxRate.Queries;
using EcommercePos.Application.Features.TaxRate.Commands;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class TaxRateEndpoints
{
    public static void MapTaxRateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tax-rates").WithTags("TaxRates");

        group.MapGet("/", async (
            [AsParameters] GetTaxRates.Request request,
            [FromServices] GetTaxRates.Handler handler,
            CancellationToken ct) =>
        {
            var query = new GetTaxRates.Query(request.PageIndex, request.PageSize, request.Search);
            var result = await handler.Handle(query, ct);
            return result.ToHttpResult();
        })
        .WithName("GetTaxRates")
        .WithSummary("Get paginated tax rates");

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] GetTaxRateById.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetTaxRateById.Query(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetTaxRateById")
        .WithSummary("Get tax rate by id");

        group.MapPost("/", async (
            [FromBody] CreateTaxRate.Request request,
            [FromServices] CreateTaxRate.Handler handler,
            CancellationToken ct) =>
        {
            var command = new CreateTaxRate.Command(
                request.TaxName, request.TaxRate, request.TaxCode, request.Description,
                request.IsActive, request.TaxType, request.IsPercentage, request.IsInclusive,
                request.IsDefault, request.Country, request.ApplyToShipping, request.Priority,
                request.EffectiveFrom, request.EffectiveTo);
            var result = await handler.Handle(command, ct);
            return result.ToCreatedResult($"/api/tax-rates/{request.TaxName}");
        })
        .WithName("CreateTaxRate")
        .WithSummary("Create a new tax rate");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateTaxRate.Request request,
            [FromServices] UpdateTaxRate.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateTaxRate.Command(
                id, request.TaxName, request.TaxRate, request.TaxCode, request.Description,
                request.IsActive, request.TaxType, request.IsPercentage, request.IsInclusive,
                request.IsDefault, request.Country, request.ApplyToShipping, request.Priority,
                request.EffectiveFrom, request.EffectiveTo);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateTaxRate")
        .WithSummary("Update an existing tax rate");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] DeleteTaxRate.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteTaxRate.Command(id), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteTaxRate")
        .WithSummary("Soft delete a tax rate");
    }
}
