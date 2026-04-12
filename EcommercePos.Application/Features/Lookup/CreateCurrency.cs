using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class CreateCurrency
{
    public sealed record Request(string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);
    public sealed record Response(string CurrencyCode, string Name);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(3);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Symbol).NotEmpty().MaximumLength(5);
            RuleFor(x => x.ExchangeRate).GreaterThan(0);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.Currencies.AnyAsync(c => c.CurrencyCode == request.CurrencyCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Currency '{request.CurrencyCode}' already exists."));

            var entity = new Currencies
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

            _context.Currencies.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.CurrencyCode, entity.Name));
        }
    }
}
