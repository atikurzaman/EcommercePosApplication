using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;

namespace EcommercePos.Api.Endpoints;

public static class CurrencyEndpoints
{
    public static void MapCurrencyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/currencies").WithTags("Currencies");

        group.MapGet("/", async (
            [AsParameters] GetCurrenciesRequest request,
            ApplicationDbContext context,
            CancellationToken ct) =>
        {
            var query = context.Currencies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.Name.Contains(request.Search) || c.CurrencyCode.Contains(request.Search));
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.CurrencyCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CurrencyResponse(
                    c.CurrencyCode, c.Name, c.Symbol, c.ExchangeRate, c.DecimalPlaces, c.IsBaseCurrency, c.IsActive))
                .ToListAsync(ct);

            return Results.Ok(new { data = items, totalCount });
        })
        .WithName("GetCurrencies")
        .WithSummary("Get paginated currencies");

        group.MapGet("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var currency = await context.Currencies
                .Where(c => c.CurrencyCode == code)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (currency == null)
                return Results.NotFound(new { error = "Currency not found" });

            return Results.Ok(new { data = new CurrencyResponse(
                currency.CurrencyCode, currency.Name, currency.Symbol, currency.ExchangeRate, 
                currency.DecimalPlaces, currency.IsBaseCurrency, currency.IsActive) });
        })
        .WithName("GetCurrencyByCode")
        .WithSummary("Get currency by code");

        group.MapPost("/", async (CreateCurrencyRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var exists = await context.Currencies.AnyAsync(c => c.CurrencyCode == request.CurrencyCode, ct);
            if (exists)
                return Results.Conflict(new { error = "Currency code already exists" });

            var currency = new Currencies
            {
                CurrencyCode = request.CurrencyCode,
                Name = request.Name,
                Symbol = request.Symbol,
                ExchangeRate = request.ExchangeRate,
                DecimalPlaces = request.DecimalPlaces,
                IsBaseCurrency = request.IsBaseCurrency,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            context.Currencies.Add(currency);
            await context.SaveChangesAsync(ct);

            return Results.Created($"/api/currencies/{currency.CurrencyCode}", new { data = new CurrencyResponse(
                currency.CurrencyCode, currency.Name, currency.Symbol, currency.ExchangeRate,
                currency.DecimalPlaces, currency.IsBaseCurrency, currency.IsActive) });
        })
        .WithName("CreateCurrency")
        .WithSummary("Create a new currency");

        group.MapPut("/{code}", async (string code, UpdateCurrencyRequest request, ApplicationDbContext context, CancellationToken ct) =>
        {
            var currency = await context.Currencies.FirstOrDefaultAsync(c => c.CurrencyCode == code, ct);
            if (currency == null)
                return Results.NotFound(new { error = "Currency not found" });

            if (currency.CurrencyCode != request.CurrencyCode)
            {
                var exists = await context.Currencies.AnyAsync(c => c.CurrencyCode == request.CurrencyCode, ct);
                if (exists)
                    return Results.Conflict(new { error = "Currency code already exists" });
            }

            currency.CurrencyCode = request.CurrencyCode;
            currency.Name = request.Name;
            currency.Symbol = request.Symbol;
            currency.ExchangeRate = request.ExchangeRate;
            currency.DecimalPlaces = request.DecimalPlaces;
            currency.IsBaseCurrency = request.IsBaseCurrency;
            currency.IsActive = request.IsActive;
            currency.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return Results.Ok(new { data = new CurrencyResponse(
                currency.CurrencyCode, currency.Name, currency.Symbol, currency.ExchangeRate,
                currency.DecimalPlaces, currency.IsBaseCurrency, currency.IsActive) });
        })
        .WithName("UpdateCurrency")
        .WithSummary("Update an existing currency");

        group.MapDelete("/{code}", async (string code, ApplicationDbContext context, CancellationToken ct) =>
        {
            var currency = await context.Currencies.FirstOrDefaultAsync(c => c.CurrencyCode == code, ct);
            if (currency == null)
                return Results.NotFound(new { error = "Currency not found" });

            context.Currencies.Remove(currency);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        })
        .WithName("DeleteCurrency")
        .WithSummary("Delete a currency");
    }
}

public record GetCurrenciesRequest(int PageIndex = 0, int PageSize = 10, string? Search = null);
public record CurrencyResponse(
    string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);
public record CreateCurrencyRequest(
    string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);
public record UpdateCurrencyRequest(
    string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);
