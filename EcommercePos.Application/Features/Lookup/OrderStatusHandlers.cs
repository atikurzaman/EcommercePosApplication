using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetOrderStatuses
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.OrderStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.DisplayName.Contains(request.Search) || c.StatusCode.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.StatusCode, c.DisplayName, c.Description, c.SortOrder, c.IsTerminal))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetOrderStatusByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.OrderStatuses.AsNoTracking()
                .Where(c => c.StatusCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Order status not found."));

            return Result<Response>.Success(new Response(entity.StatusCode, entity.DisplayName, entity.Description, entity.SortOrder, entity.IsTerminal));
        }
    }
}

public static class CreateOrderStatus
{
    public sealed record Request(string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);
    public sealed record Response(string StatusCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.OrderStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Order status '{request.StatusCode}' already exists."));

            var entity = new OrderStatuses
            {
                StatusCode = request.StatusCode,
                DisplayName = request.DisplayName,
                Description = request.Description,
                SortOrder = request.SortOrder,
                IsTerminal = request.IsTerminal
            };

            _context.OrderStatuses.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.StatusCode, entity.DisplayName));
        }
    }
}

public static class UpdateOrderStatus
{
    public sealed record Request(string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);
    public sealed record Command(string OriginalCode, string StatusCode, string DisplayName, string? Description, byte SortOrder, bool IsTerminal);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetOrderStatusByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.OrderStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetOrderStatusByCode.Response>.Failure(Error.NotFound("Order status not found."));

            if (entity.StatusCode != command.StatusCode)
            {
                var exists = await _context.OrderStatuses.AnyAsync(c => c.StatusCode == command.StatusCode, ct);
                if (exists)
                    return Result<GetOrderStatusByCode.Response>.Failure(Error.Conflict($"Order status '{command.StatusCode}' already exists."));
            }

            entity.StatusCode = command.StatusCode;
            entity.DisplayName = command.DisplayName;
            entity.Description = command.Description;
            entity.SortOrder = command.SortOrder;
            entity.IsTerminal = command.IsTerminal;

            await _context.SaveChangesAsync(ct);
            return Result<GetOrderStatusByCode.Response>.Success(
                new GetOrderStatusByCode.Response(entity.StatusCode, entity.DisplayName, entity.Description, entity.SortOrder, entity.IsTerminal));
        }
    }
}

public static class DeleteOrderStatus
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.OrderStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Order status not found."));

            _context.OrderStatuses.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
