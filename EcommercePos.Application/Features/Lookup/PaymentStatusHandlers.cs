using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetPaymentStatuses
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string StatusCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.PaymentStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.StatusCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.StatusCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.StatusCode, c.DisplayName))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetPaymentStatusByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string StatusCode, string DisplayName);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.PaymentStatuses.AsNoTracking()
                .Where(c => c.StatusCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Payment status not found."));

            return Result<Response>.Success(new Response(entity.StatusCode, entity.DisplayName));
        }
    }
}

public static class CreatePaymentStatus
{
    public sealed record Request(string StatusCode, string DisplayName);
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
            var exists = await _context.PaymentStatuses.AnyAsync(c => c.StatusCode == request.StatusCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Payment status '{request.StatusCode}' already exists."));

            var entity = new PaymentStatuses
            {
                StatusCode = request.StatusCode,
                DisplayName = request.DisplayName
            };

            _context.PaymentStatuses.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.StatusCode, entity.DisplayName));
        }
    }
}

public static class UpdatePaymentStatus
{
    public sealed record Request(string StatusCode, string DisplayName);
    public sealed record Command(string OriginalCode, string StatusCode, string DisplayName);

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

        public async Task<Result<GetPaymentStatusByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PaymentStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetPaymentStatusByCode.Response>.Failure(Error.NotFound("Payment status not found."));

            if (entity.StatusCode != command.StatusCode)
            {
                var exists = await _context.PaymentStatuses.AnyAsync(c => c.StatusCode == command.StatusCode, ct);
                if (exists)
                    return Result<GetPaymentStatusByCode.Response>.Failure(Error.Conflict($"Payment status '{command.StatusCode}' already exists."));
            }

            entity.StatusCode = command.StatusCode;
            entity.DisplayName = command.DisplayName;

            await _context.SaveChangesAsync(ct);
            return Result<GetPaymentStatusByCode.Response>.Success(
                new GetPaymentStatusByCode.Response(entity.StatusCode, entity.DisplayName));
        }
    }
}

public static class DeletePaymentStatus
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PaymentStatuses.FirstOrDefaultAsync(c => c.StatusCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Payment status not found."));

            _context.PaymentStatuses.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
