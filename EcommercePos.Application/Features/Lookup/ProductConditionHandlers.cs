using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetProductConditions
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string ConditionCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.ProductConditions.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.ConditionCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.ConditionCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.ConditionCode, c.DisplayName))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetProductConditionByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string ConditionCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.ProductConditions.AsNoTracking()
                .Where(c => c.ConditionCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Product condition not found."));

            return Result<Response>.Success(new Response(entity.ConditionCode, entity.DisplayName));
        }
    }
}

public static class CreateProductCondition
{
    public sealed record Request(string ConditionCode, string DisplayName);
    public sealed record Response(string ConditionCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ConditionCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.ProductConditions.AnyAsync(c => c.ConditionCode == request.ConditionCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Product condition '{request.ConditionCode}' already exists."));

            var entity = new ProductConditions
            {
                ConditionCode = request.ConditionCode,
                DisplayName = request.DisplayName
            };

            _context.ProductConditions.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.ConditionCode, entity.DisplayName));
        }
    }
}

public static class UpdateProductCondition
{
    public sealed record Request(string ConditionCode, string DisplayName);
    public sealed record Command(string OriginalCode, string ConditionCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ConditionCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetProductConditionByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ProductConditions.FirstOrDefaultAsync(c => c.ConditionCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetProductConditionByCode.Response>.Failure(Error.NotFound("Product condition not found."));

            if (entity.ConditionCode != command.ConditionCode)
            {
                var exists = await _context.ProductConditions.AnyAsync(c => c.ConditionCode == command.ConditionCode, ct);
                if (exists)
                    return Result<GetProductConditionByCode.Response>.Failure(Error.Conflict($"Product condition '{command.ConditionCode}' already exists."));
            }

            entity.ConditionCode = command.ConditionCode;
            entity.DisplayName = command.DisplayName;

            await _context.SaveChangesAsync(ct);
            return Result<GetProductConditionByCode.Response>.Success(
                new GetProductConditionByCode.Response(entity.ConditionCode, entity.DisplayName));
        }
    }
}

public static class DeleteProductCondition
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ProductConditions.FirstOrDefaultAsync(c => c.ConditionCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Product condition not found."));

            _context.ProductConditions.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
