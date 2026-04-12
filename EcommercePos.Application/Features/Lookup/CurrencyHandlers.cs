using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetCurrencies
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Currencies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.Name.Contains(request.Search) || c.CurrencyCode.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.CurrencyCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.CurrencyCode, c.Name, c.Symbol, c.ExchangeRate, c.DecimalPlaces, c.IsBaseCurrency, c.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetCurrencyByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string CurrencyCode, string Name, string Symbol, decimal ExchangeRate, byte DecimalPlaces, bool IsBaseCurrency, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Currencies.AsNoTracking()
                .Where(c => c.CurrencyCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Currency not found."));

            return Result<Response>.Success(new Response(entity.CurrencyCode, entity.Name, entity.Symbol, entity.ExchangeRate, entity.DecimalPlaces, entity.IsBaseCurrency, entity.IsActive));
        }
    }
}

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
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

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

public static class DeleteCurrency
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencyCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Currency not found."));

            _context.Currencies.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
