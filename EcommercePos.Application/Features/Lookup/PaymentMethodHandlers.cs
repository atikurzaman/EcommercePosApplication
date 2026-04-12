using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetPaymentMethods
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.PaymentMethods.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.MethodCode.Contains(request.Search) || c.DisplayName.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.MethodCode, c.DisplayName, c.IsOnline, c.IsActive, c.SortOrder))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetPaymentMethodByCode
{
    public sealed record Query(string Code);
    public sealed record Response(string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.PaymentMethods.AsNoTracking()
                .Where(c => c.MethodCode == query.Code)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Payment method not found."));

            return Result<Response>.Success(new Response(entity.MethodCode, entity.DisplayName, entity.IsOnline, entity.IsActive, entity.SortOrder));
        }
    }
}

public static class CreatePaymentMethod
{
    public sealed record Request(string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);
    public sealed record Response(string MethodCode, string DisplayName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.MethodCode).NotEmpty().MaximumLength(40);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var exists = await _context.PaymentMethods.AnyAsync(c => c.MethodCode == request.MethodCode, ct);
            if (exists)
                return Result<Response>.Failure(Error.Conflict($"Payment method '{request.MethodCode}' already exists."));

            var entity = new PaymentMethods
            {
                MethodCode = request.MethodCode,
                DisplayName = request.DisplayName,
                IsOnline = request.IsOnline,
                IsActive = request.IsActive,
                SortOrder = request.SortOrder
            };

            _context.PaymentMethods.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.MethodCode, entity.DisplayName));
        }
    }
}

public static class UpdatePaymentMethod
{
    public sealed record Request(string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);
    public sealed record Command(string OriginalCode, string MethodCode, string DisplayName, bool IsOnline, bool IsActive, byte SortOrder);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.MethodCode).NotEmpty().MaximumLength(40);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(80);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetPaymentMethodByCode.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PaymentMethods.FirstOrDefaultAsync(c => c.MethodCode == command.OriginalCode, ct);
            if (entity == null)
                return Result<GetPaymentMethodByCode.Response>.Failure(Error.NotFound("Payment method not found."));

            if (entity.MethodCode != command.MethodCode)
            {
                var exists = await _context.PaymentMethods.AnyAsync(c => c.MethodCode == command.MethodCode, ct);
                if (exists)
                    return Result<GetPaymentMethodByCode.Response>.Failure(Error.Conflict($"Payment method '{command.MethodCode}' already exists."));
            }

            entity.MethodCode = command.MethodCode;
            entity.DisplayName = command.DisplayName;
            entity.IsOnline = command.IsOnline;
            entity.IsActive = command.IsActive;
            entity.SortOrder = command.SortOrder;

            await _context.SaveChangesAsync(ct);
            return Result<GetPaymentMethodByCode.Response>.Success(
                new GetPaymentMethodByCode.Response(entity.MethodCode, entity.DisplayName, entity.IsOnline, entity.IsActive, entity.SortOrder));
        }
    }
}

public static class DeletePaymentMethod
{
    public sealed record Command(string Code);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PaymentMethods.FirstOrDefaultAsync(c => c.MethodCode == command.Code, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Payment method not found."));

            _context.PaymentMethods.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
