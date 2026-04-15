using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class UpdateCurrency
{
    public sealed record Request(string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);
    public sealed record Command(string OriginalCode, string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<GetCurrencyByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencyCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetCurrencyByCode.Response>.Failure(Error.NotFound("Currency not found."));

            if (entity.CurrencyCode != command.CurrencyCode)
            {
                var exists = await _context.Currencies.AnyAsync(c => c.CurrencyCode == command.CurrencyCode, ct);
                if (exists)
                    return Result<GetCurrencyByCode.Response>.Failure(Error.Conflict($"Currency '{command.CurrencyCode}' already exists."));
            }

            entity.CurrencyCode = command.CurrencyCode;
            entity.Name = command.Name;
            entity.Symbol = command.Symbol;
            entity.ExchangeRate = command.ExchangeRate;
            entity.DecimalPlaces = command.DecimalPlaces;
            entity.IsBaseCurrency = command.IsBaseCurrency;
            entity.IsActive = command.IsActive;

            await _context.SaveChangesAsync(ct);
            return Result<GetCurrencyByCode.Response>.Success(
                new GetCurrencyByCode.Response(entity.CurrencyCode, entity.Name, entity.Symbol, entity.ExchangeRate, entity.DecimalPlaces, entity.IsBaseCurrency, entity.IsActive));
        }
    }
}
