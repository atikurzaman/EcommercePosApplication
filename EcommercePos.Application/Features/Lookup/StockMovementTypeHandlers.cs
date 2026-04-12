using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetStockMovementTypes
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string TypeCode, string DisplayName, bool IsInbound);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.StockMovementTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.TypeCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.TypeCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.TypeCode, c.DisplayName, c.IsInbound))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetStockMovementTypeByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string TypeCode, string DisplayName, bool IsInbound);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.StockMovementTypes.AsNoTracking()
                .Where(c => c.TypeCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Stock movement type not found."));

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName, entity.IsInbound));
        }
    }
}

public static class CreateStockMovementType
{
    public sealed record Request(string TypeCode, string DisplayName, bool IsInbound);
    public sealed record Response(string TypeCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.StockMovementTypes.AnyAsync(c => c.TypeCode == request.TypeCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Stock movement type '{request.TypeCode}' already exists."));

            var entity = new StockMovementTypes
            {
                TypeCode = request.TypeCode,
                DisplayName = request.DisplayName,
                IsInbound = request.IsInbound
            };

            _context.StockMovementTypes.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName));
        }
    }
}

public static class UpdateStockMovementType
{
    public sealed record Request(string TypeCode, string DisplayName, bool IsInbound);
    public sealed record Command(string OriginalCode, string TypeCode, string DisplayName, bool IsInbound);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetStockMovementTypeByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.StockMovementTypes.FirstOrDefaultAsync(c => c.TypeCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetStockMovementTypeByCode.Response>.Failure(Error.NotFound("Stock movement type not found."));

            if (entity.TypeCode != command.TypeCode)
            {
                var exists = await _context.StockMovementTypes.AnyAsync(c => c.TypeCode == command.TypeCode, ct);
                if (exists)
                    return Result<GetStockMovementTypeByCode.Response>.Failure(Error.Conflict($"Stock movement type '{command.TypeCode}' already exists."));
            }

            entity.TypeCode = command.TypeCode;
            entity.DisplayName = command.DisplayName;
            entity.IsInbound = command.IsInbound;

            await _context.SaveChangesAsync(ct);
            return Result<GetStockMovementTypeByCode.Response>.Success(
                new GetStockMovementTypeByCode.Response(entity.TypeCode, entity.DisplayName, entity.IsInbound));
        }
    }
}

public static class DeleteStockMovementType
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.StockMovementTypes.FirstOrDefaultAsync(c => c.TypeCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Stock movement type not found."));

            _context.StockMovementTypes.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
