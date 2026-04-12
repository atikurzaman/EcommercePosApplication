using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Lookup;

public static class GetColors
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null);
    public sealed record Response(Guid Id, string Name, string? HexCode, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.Colors.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.Name.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.Name)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(c.Id, c.Name, c.HexCode, c.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

public static class GetColorById
{
    public sealed record Query(Guid Id);
    public sealed record Response(Guid Id, string Name, string? HexCode, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.Colors.AsNoTracking()
                .Where(c => c.Id == query.Id)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("Color not found."));

            return Result<Response>.Success(new Response(entity.Id, entity.Name, entity.HexCode, entity.IsActive));
        }
    }
}

public static class CreateColor
{
    public sealed record Request(string Name, string? HexCode, bool IsActive);
    public sealed record Response(Guid Id, string Name);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.HexCode).MaximumLength(7).Matches(@"^#[0-9A-Fa-f]{6}$").When(x => x.HexCode != null);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var entity = new Colors
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                HexCode = request.HexCode,
                IsActive = request.IsActive,
                IsDeleted = false
            };

            _context.Colors.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.Name));
        }
    }
}

public static class UpdateColor
{
    public sealed record Request(string Name, string? HexCode, bool IsActive);
    public sealed record Command(Guid Id, string Name, string? HexCode, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.HexCode).MaximumLength(7).Matches(@"^#[0-9A-Fa-f]{6}$").When(x => x.HexCode != null);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetColorById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Colors.FirstOrDefaultAsync(c => c.Id == command.Id, ct);
            if (entity == null)
                return Result<GetColorById.Response>.Failure(Error.NotFound("Color not found."));

            entity.Name = command.Name;
            entity.HexCode = command.HexCode;
            entity.IsActive = command.IsActive;

            await _context.SaveChangesAsync(ct);
            return Result<GetColorById.Response>.Success(
                new GetColorById.Response(entity.Id, entity.Name, entity.HexCode, entity.IsActive));
        }
    }
}

public static class DeleteColor
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.Colors.FirstOrDefaultAsync(c => c.Id == command.Id, ct);
            if (entity == null)
                return Result.Failure(Error.NotFound("Color not found."));

            entity.IsDeleted = true;
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
