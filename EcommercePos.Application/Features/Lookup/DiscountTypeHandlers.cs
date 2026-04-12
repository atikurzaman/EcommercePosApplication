using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetDiscountTypes
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string TypeCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.DiscountTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.TypeCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.TypeCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.TypeCode, c.DisplayName))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetDiscountTypeByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string TypeCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.DiscountTypes.AsNoTracking()
                .Where(c => c.TypeCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Discount type not found."));

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName));
        }
    }
}

public static class CreateDiscountType
{
    public sealed record Request(string TypeCode, string DisplayName);
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
            var exists = await _context.DiscountTypes.AnyAsync(c => c.TypeCode == request.TypeCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Discount type '{request.TypeCode}' already exists."));

            var entity = new DiscountTypes
            {
                TypeCode = request.TypeCode,
                DisplayName = request.DisplayName
            };

            _context.DiscountTypes.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName));
        }
    }
}

public static class UpdateDiscountType
{
    public sealed record Request(string TypeCode, string DisplayName);
    public sealed record Command(string OriginalCode, string TypeCode, string DisplayName);

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

        public async Task<Result<GetDiscountTypeByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.DiscountTypes.FirstOrDefaultAsync(c => c.TypeCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetDiscountTypeByCode.Response>.Failure(Error.NotFound("Discount type not found."));

            if (entity.TypeCode != command.TypeCode)
            {
                var exists = await _context.DiscountTypes.AnyAsync(c => c.TypeCode == command.TypeCode, ct);
                if (exists)
                    return Result<GetDiscountTypeByCode.Response>.Failure(Error.Conflict($"Discount type '{command.TypeCode}' already exists."));
            }

            entity.TypeCode = command.TypeCode;
            entity.DisplayName = command.DisplayName;

            await _context.SaveChangesAsync(ct);
            return Result<GetDiscountTypeByCode.Response>.Success(
                new GetDiscountTypeByCode.Response(entity.TypeCode, entity.DisplayName));
        }
    }
}

public static class DeleteDiscountType
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.DiscountTypes.FirstOrDefaultAsync(c => c.TypeCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Discount type not found."));

            _context.DiscountTypes.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
