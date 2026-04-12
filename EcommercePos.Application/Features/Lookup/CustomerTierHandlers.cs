using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetCustomerTiers
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.CustomerTiers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.TierCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.TierCode, c.DisplayName, c.MinLifetimeSpend, c.DiscountPct, c.PointsMultiplier, c.SortOrder))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetCustomerTierByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.CustomerTiers.AsNoTracking()
                .Where(c => c.TierCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Customer tier not found."));

            return Result<Response>.Success(new Response(entity.TierCode, entity.DisplayName, entity.MinLifetimeSpend, entity.DiscountPct, entity.PointsMultiplier, entity.SortOrder));
        }
    }
}

public static class CreateCustomerTier
{
    public sealed record Request(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);
    public sealed record Response(string TierCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TierCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.CustomerTiers.AnyAsync(c => c.TierCode == request.TierCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Customer tier '{request.TierCode}' already exists."));

            var entity = new CustomerTiers
            {
                TierCode = request.TierCode,
                DisplayName = request.DisplayName,
                MinLifetimeSpend = request.MinLifetimeSpend,
                DiscountPct = request.DiscountPct,
                PointsMultiplier = request.PointsMultiplier,
                SortOrder = request.SortOrder
            };

            _context.CustomerTiers.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.TierCode, entity.DisplayName));
        }
    }
}

public static class UpdateCustomerTier
{
    public sealed record Request(string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);
    public sealed record Command(string OriginalCode, string TierCode, string DisplayName, decimal MinLifetimeSpend, decimal DiscountPct, decimal PointsMultiplier, byte SortOrder);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TierCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetCustomerTierByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.CustomerTiers.FirstOrDefaultAsync(c => c.TierCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetCustomerTierByCode.Response>.Failure(Error.NotFound("Customer tier not found."));

            if (entity.TierCode != command.TierCode)
            {
                var exists = await _context.CustomerTiers.AnyAsync(c => c.TierCode == command.TierCode, ct);
                if (exists)
                    return Result<GetCustomerTierByCode.Response>.Failure(Error.Conflict($"Customer tier '{command.TierCode}' already exists."));
            }

            entity.TierCode = command.TierCode;
            entity.DisplayName = command.DisplayName;
            entity.MinLifetimeSpend = command.MinLifetimeSpend;
            entity.DiscountPct = command.DiscountPct;
            entity.PointsMultiplier = command.PointsMultiplier;
            entity.SortOrder = command.SortOrder;

            await _context.SaveChangesAsync(ct);
            return Result<GetCustomerTierByCode.Response>.Success(
                new GetCustomerTierByCode.Response(entity.TierCode, entity.DisplayName, entity.MinLifetimeSpend, entity.DiscountPct, entity.PointsMultiplier, entity.SortOrder));
        }
    }
}

public static class DeleteCustomerTier
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.CustomerTiers.FirstOrDefaultAsync(c => c.TierCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Customer tier not found."));

            _context.CustomerTiers.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
