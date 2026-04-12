using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetReturnStatuses
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string StatusCode, string DisplayName, byte SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.ReturnStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.StatusCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.StatusCode, c.DisplayName, c.SortOrder))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetReturnStatusByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string StatusCode, string DisplayName, byte SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.ReturnStatuses.AsNoTracking()
                .Where(c => c.StatusCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Return status not found."));

            return Result<Response>.Success(new Response(entity.StatusCode, entity.DisplayName, entity.SortOrder));
        }
    }
}

public static class CreateReturnStatus
{
    public sealed record Request(string StatusCode, string DisplayName, byte SortOrder);
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
            var exists = await _context.ReturnStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Return status '{request.StatusCode}' already exists."));

            var entity = new ReturnStatuses
            {
                StatusCode = request.StatusCode,
                DisplayName = request.DisplayName,
                SortOrder = request.SortOrder
            };

            _context.ReturnStatuses.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.StatusCode, entity.DisplayName));
        }
    }
}

public static class UpdateReturnStatus
{
    public sealed record Request(string StatusCode, string DisplayName, byte SortOrder);
    public sealed record Command(string OriginalCode, string StatusCode, string DisplayName, byte SortOrder);

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

        public async Task<Result<GetReturnStatusByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ReturnStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetReturnStatusByCode.Response>.Failure(Error.NotFound("Return status not found."));

            if (entity.StatusCode != command.StatusCode)
            {
                var exists = await _context.ReturnStatuses.AnyAsync(c => c.StatusCode == command.StatusCode, ct);
                if (exists)
                    return Result<GetReturnStatusByCode.Response>.Failure(Error.Conflict($"Return status '{command.StatusCode}' already exists."));
            }

            entity.StatusCode = command.StatusCode;
            entity.DisplayName = command.DisplayName;
            entity.SortOrder = command.SortOrder;

            await _context.SaveChangesAsync(ct);
            return Result<GetReturnStatusByCode.Response>.Success(
                new GetReturnStatusByCode.Response(entity.StatusCode, entity.DisplayName, entity.SortOrder));
        }
    }
}

public static class DeleteReturnStatus
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.ReturnStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Return status not found."));

            _context.ReturnStatuses.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
