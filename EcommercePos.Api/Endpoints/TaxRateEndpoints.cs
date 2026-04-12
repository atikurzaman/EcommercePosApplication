using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.TaxRate;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class TaxRateEndpoints
{
    public static void MapTaxRateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tax-rates").WithTags("TaxRates");

        group.MapGet("/", async (
            [AsParameters] GetTaxRates.Query query,
            GetTaxRates.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(query, ct)).ToPagedResult())
            .WithName("GetTaxRates")
            .WithSummary("Get paginated tax rates");

        group.MapGet("/{id:guid}", async (
            Guid id,
            GetTaxRateById.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new GetTaxRateById.Query(id), ct)).ToHttpResult())
            .WithName("GetTaxRateById")
            .WithSummary("Get tax rate by id");

        group.MapPost("/", async (
            [FromBody] CreateTaxRate.Command command,
            CreateTaxRate.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(command, ct)).ToCreatedResult("/api/tax-rates"))
            .WithName("CreateTaxRate")
            .WithSummary("Create a new tax rate");

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateTaxRateBody body,
            UpdateTaxRate.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new UpdateTaxRate.Command(
                id, body.TaxName, body.TaxRate, body.TaxCode, body.Description,
                body.IsActive, body.TaxType, body.IsPercentage, body.IsInclusive,
                body.IsDefault, body.Country, body.ApplyToShipping, body.Priority,
                body.EffectiveFrom, body.EffectiveTo), ct)).ToHttpResult())
            .WithName("UpdateTaxRate")
            .WithSummary("Update an existing tax rate");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            DeleteTaxRate.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(new DeleteTaxRate.Command(id), ct)).ToNoContentResult())
            .WithName("DeleteTaxRate")
            .WithSummary("Soft delete a tax rate");
    }
}

public record UpdateTaxRateBody(
    string TaxName, decimal TaxRate, string? TaxCode, string? Description,
    bool IsActive, string? TaxType, bool IsPercentage, bool IsInclusive,
    bool IsDefault, string? Country, bool ApplyToShipping, int Priority,
    DateTime? EffectiveFrom, DateTime? EffectiveTo);
