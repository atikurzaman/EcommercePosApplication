using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetWishlistTypes
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string TypeCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.WishlistTypes.AsNoTracking();

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

public static class GetWishlistTypeByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string TypeCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.WishlistTypes.AsNoTracking()
                .Where(c => c.TypeCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Wishlist type not found."));

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName));
        }
    }
}

public static class CreateWishlistType
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
            var exists = await _context.WishlistTypes.AnyAsync(c => c.TypeCode == request.TypeCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Wishlist type '{request.TypeCode}' already exists."));

            var entity = new WishlistTypes
            {
                TypeCode = request.TypeCode,
                DisplayName = request.DisplayName
            };

            _context.WishlistTypes.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.TypeCode, entity.DisplayName));
        }
    }
}

public static class UpdateWishlistType
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

        public async Task<Result<GetWishlistTypeByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.WishlistTypes.FirstOrDefaultAsync(c => c.TypeCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetWishlistTypeByCode.Response>.Failure(Error.NotFound("Wishlist type not found."));

            if (entity.TypeCode != command.TypeCode)
            {
                var exists = await _context.WishlistTypes.AnyAsync(c => c.TypeCode == command.TypeCode, ct);
                if (exists)
                    return Result<GetWishlistTypeByCode.Response>.Failure(Error.Conflict($"Wishlist type '{command.TypeCode}' already exists."));
            }

            entity.TypeCode = command.TypeCode;
            entity.DisplayName = command.DisplayName;

            await _context.SaveChangesAsync(ct);
            return Result<GetWishlistTypeByCode.Response>.Success(
                new GetWishlistTypeByCode.Response(entity.TypeCode, entity.DisplayName));
        }
    }
}

public static class DeleteWishlistType
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.WishlistTypes.FirstOrDefaultAsync(c => c.TypeCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Wishlist type not found."));

            _context.WishlistTypes.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
