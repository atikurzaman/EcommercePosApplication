using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosTerminals ────────────────────────────────────────────────────────────
public static class GetPosTerminals
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, Guid? PosCounterId = null, string? Search = null);

    public sealed record Response(
        Guid Id, Guid PosCounterId, string CounterName,
        string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.PosTerminals
                .AsNoTracking()
                .Where(t => !t.IsDeleted);

            if (request.PosCounterId.HasValue)
                query = query.Where(t => t.PosCounterId == request.PosCounterId.Value);
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(t => t.TerminalName.Contains(request.Search) || t.TerminalCode.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(t => t.TerminalCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new Response(
                    t.Id, t.PosCounterId, t.PosCounter.CounterName,
                    t.TerminalCode, t.TerminalName,
                    t.MachineName, t.Ipaddress, t.PrinterName, t.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

// ── GetPosTerminalById ─────────────────────────────────────────────────────────
public static class GetPosTerminalById
{
    public sealed record Query(Guid Id);

    public sealed record Response(
        Guid Id, Guid PosCounterId, string CounterName,
        string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.PosTerminals
                .AsNoTracking()
                .Where(t => t.Id == query.Id && !t.IsDeleted)
                .Select(t => new Response(
                    t.Id, t.PosCounterId, t.PosCounter.CounterName,
                    t.TerminalCode, t.TerminalName,
                    t.MachineName, t.Ipaddress, t.PrinterName, t.IsActive))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("POS terminal not found."));

            return Result<Response>.Success(entity);
        }
    }
}

// ── CreatePosTerminal ──────────────────────────────────────────────────────────
public static class CreatePosTerminal
{
    public sealed record Request(
        Guid PosCounterId, string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed record Response(Guid Id, string TerminalCode, string TerminalName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PosCounterId).NotEmpty();
            RuleFor(x => x.TerminalCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.TerminalName).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var counterExists = await _context.PosCounters
                .AnyAsync(c => c.Id == request.PosCounterId && !c.IsDeleted, ct);
            if (!counterExists)
                return Result<Response>.Failure(Error.NotFound("POS counter not found."));

            var codeExists = await _context.PosTerminals
                .AnyAsync(t => t.PosCounterId == request.PosCounterId
                    && t.TerminalCode == request.TerminalCode && !t.IsDeleted, ct);
            if (codeExists)
                return Result<Response>.Failure(
                    Error.Conflict($"Terminal code '{request.TerminalCode}' already exists for this counter."));

            var entity = new PosTerminals
            {
                Id = Guid.NewGuid(),
                PosCounterId = request.PosCounterId,
                TerminalCode = request.TerminalCode,
                TerminalName = request.TerminalName,
                MachineName = request.MachineName,
                Ipaddress = request.Ipaddress,
                PrinterName = request.PrinterName,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.PosTerminals.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.TerminalCode, entity.TerminalName));
        }
    }
}

// ── UpdatePosTerminal ──────────────────────────────────────────────────────────
public static class UpdatePosTerminal
{
    public sealed record Request(
        string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed record Command(
        Guid Id, string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TerminalCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.TerminalName).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetPosTerminalById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PosTerminals
                .FirstOrDefaultAsync(t => t.Id == command.Id && !t.IsDeleted, ct);

            if (entity == null)
                return Result<GetPosTerminalById.Response>.Failure(Error.NotFound("POS terminal not found."));

            if (entity.TerminalCode != command.TerminalCode)
            {
                var codeExists = await _context.PosTerminals
                    .AnyAsync(t => t.PosCounterId == entity.PosCounterId
                        && t.TerminalCode == command.TerminalCode && t.Id != command.Id && !t.IsDeleted, ct);
                if (codeExists)
                    return Result<GetPosTerminalById.Response>.Failure(
                        Error.Conflict($"Terminal code '{command.TerminalCode}' already exists for this counter."));
            }

            entity.TerminalCode = command.TerminalCode;
            entity.TerminalName = command.TerminalName;
            entity.MachineName = command.MachineName;
            entity.Ipaddress = command.Ipaddress;
            entity.PrinterName = command.PrinterName;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var counterName = await _context.PosCounters
                .Where(c => c.Id == entity.PosCounterId)
                .Select(c => c.CounterName)
                .FirstOrDefaultAsync(ct) ?? string.Empty;

            return Result<GetPosTerminalById.Response>.Success(
                new GetPosTerminalById.Response(
                    entity.Id, entity.PosCounterId, counterName,
                    entity.TerminalCode, entity.TerminalName,
                    entity.MachineName, entity.Ipaddress, entity.PrinterName, entity.IsActive));
        }
    }
}

// ── DeletePosTerminal ──────────────────────────────────────────────────────────
public static class DeletePosTerminal
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PosTerminals
                .FirstOrDefaultAsync(t => t.Id == command.Id && !t.IsDeleted, ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("POS terminal not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
