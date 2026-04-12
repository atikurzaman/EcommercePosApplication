using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Lookup;
using EcommercePos.Api.Extensions;
using EcommercePos.Api.Filters;

namespace EcommercePos.Api.Endpoints;

public static class CurrencyEndpoints
{
    public static void MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/currencies").WithTags("Currencies");

        group.MapGet("/", async (
            [AsParameters] GetCurrencies.Request request,
            [FromServices] GetCurrencies.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToPagedResult();
        })
        .WithName("GetCurrencies")
        .WithSummary("Get paginated currencies");

        group.MapGet("/{code}", async (
            string code,
            [FromServices] GetCurrencyByCode.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new GetCurrencyByCode.Query(code), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCurrencyByCode")
        .WithSummary("Get currency by code");

        group.MapPost("/", async (
            [FromBody] CreateCurrency.Request request,
            [FromServices] CreateCurrency.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(request, ct);
            return result.ToCreatedResult($"/api/currencies/{request.CurrencyCode}");
        })
        .AddEndpointFilter<ValidationFilter<CreateCurrency.Request>>()
        .WithName("CreateCurrency")
        .WithSummary("Create a new currency");

        group.MapPut("/{code}", async (
            string code,
            [FromBody] UpdateCurrency.Request request,
            [FromServices] UpdateCurrency.Handler handler,
            CancellationToken ct) =>
        {
            var command = new UpdateCurrency.Command(code, request.CurrencyCode, request.Name, request.Symbol, request.ExchangeRate, request.DecimalPlaces, request.IsBaseCurrency, request.IsActive);
            var result = await handler.Handle(command, ct);
            return result.ToHttpResult();
        })
        .AddEndpointFilter<ValidationFilter<UpdateCurrency.Request>>()
        .WithName("UpdateCurrency")
        .WithSummary("Update an existing currency");

        group.MapDelete("/{code}", async (
            string code,
            [FromServices] DeleteCurrency.Handler handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new DeleteCurrency.Command(code), ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteCurrency")
        .WithSummary("Delete a currency");
    }
}
