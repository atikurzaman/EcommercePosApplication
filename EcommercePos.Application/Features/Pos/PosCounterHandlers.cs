using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosCounters ─────────────────────────────────────────────────────────────
public static class GetPosCounters
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, Guid? WarehouseId = null, string? Search = null);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        string CounterCode, string CounterName, bool IsActive);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<PagedResult<Response>>> Handle(Request request, CancellationToken ct)
        {
            var query = _context.PosCounters
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (request.WarehouseId.HasValue)
                query = query.Where(c => c.WarehouseId == request.WarehouseId.Value);
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(c => c.CounterName.Contains(request.Search) || c.CounterCode.Contains(request.Search));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderBy(c => c.CounterCode)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new Response(
                    c.Id, c.WarehouseId, c.Warehouse.Name,
                    c.CounterCode, c.CounterName, c.IsActive))
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(
                new PagedResult<Response>(items, totalCount, request.PageIndex, request.PageSize));
        }
    }
}

// ── GetPosCounterById ──────────────────────────────────────────────────────────
public static class GetPosCounterById
{
    public sealed record Query(Guid Id);

    public sealed record PosTerminalInfo(
        Guid Id, string TerminalCode, string TerminalName,
        string? MachineName, string? Ipaddress, string? PrinterName, bool IsActive);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        string CounterCode, string CounterName, bool IsActive,
        List<PosTerminalInfo> Terminals);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var entity = await _context.PosCounters
                .AsNoTracking()
                .Where(c => c.Id == query.Id && !c.IsDeleted)
                .Select(c => new Response(
                    c.Id, c.WarehouseId, c.Warehouse.Name,
                    c.CounterCode, c.CounterName, c.IsActive,
                    c.PosTerminals
                        .Where(t => !t.IsDeleted)
                        .OrderBy(t => t.TerminalCode)
                        .Select(t => new PosTerminalInfo(
                            t.Id, t.TerminalCode, t.TerminalName,
                            t.MachineName, t.Ipaddress, t.PrinterName, t.IsActive))
                        .ToList()))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("POS counter not found."));

            return Result<Response>.Success(entity);
        }
    }
}

// ── CreatePosCounter ───────────────────────────────────────────────────────────
public static class CreatePosCounter
{
    public sealed record Request(Guid WarehouseId, string CounterCode, string CounterName, bool IsActive);
    public sealed record Response(Guid Id, string CounterCode, string CounterName);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.CounterCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.CounterName).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
        {
            var warehouseExists = await _context.Warehouses
                .AnyAsync(w => w.Id == request.WarehouseId && !w.IsDeleted, ct);
            if (!warehouseExists)
                return Result<Response>.Failure(Error.NotFound("Warehouse not found."));

            var codeExists = await _context.PosCounters
                .AnyAsync(c => c.WarehouseId == request.WarehouseId
                    && c.CounterCode == request.CounterCode && !c.IsDeleted, ct);
            if (codeExists)
                return Result<Response>.Failure(
                    Error.Conflict($"Counter code '{request.CounterCode}' already exists in this warehouse."));

            var entity = new PosCounters
            {
                Id = Guid.NewGuid(),
                WarehouseId = request.WarehouseId,
                CounterCode = request.CounterCode,
                CounterName = request.CounterName,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.PosCounters.Add(entity);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response(entity.Id, entity.CounterCode, entity.CounterName));
        }
    }
}

// ── UpdatePosCounter ───────────────────────────────────────────────────────────
public static class UpdatePosCounter
{
    public sealed record Request(string CounterCode, string CounterName, bool IsActive);
    public sealed record Command(Guid Id, string CounterCode, string CounterName, bool IsActive);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CounterCode).NotEmpty().MaximumLength(30);
            RuleFor(x => x.CounterName).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<GetPosCounterById.Response>> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PosCounters
                .FirstOrDefaultAsync(c => c.Id == command.Id && !c.IsDeleted, ct);

            if (entity == null)
                return Result<GetPosCounterById.Response>.Failure(Error.NotFound("POS counter not found."));

            if (entity.CounterCode != command.CounterCode)
            {
                var codeExists = await _context.PosCounters
                    .AnyAsync(c => c.WarehouseId == entity.WarehouseId
                        && c.CounterCode == command.CounterCode && c.Id != command.Id && !c.IsDeleted, ct);
                if (codeExists)
                    return Result<GetPosCounterById.Response>.Failure(
                        Error.Conflict($"Counter code '{command.CounterCode}' already exists in this warehouse."));
            }

            entity.CounterCode = command.CounterCode;
            entity.CounterName = command.CounterName;
            entity.IsActive = command.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var terminals = await _context.PosTerminals
                .AsNoTracking()
                .Where(t => t.PosCounterId == entity.Id && !t.IsDeleted)
                .OrderBy(t => t.TerminalCode)
                .Select(t => new GetPosCounterById.PosTerminalInfo(
                    t.Id, t.TerminalCode, t.TerminalName,
                    t.MachineName, t.Ipaddress, t.PrinterName, t.IsActive))
                .ToListAsync(ct);

            var warehouseName = await _context.Warehouses
                .Where(w => w.Id == entity.WarehouseId)
                .Select(w => w.Name)
                .FirstOrDefaultAsync(ct) ?? string.Empty;

            return Result<GetPosCounterById.Response>.Success(
                new GetPosCounterById.Response(
                    entity.Id, entity.WarehouseId, warehouseName,
                    entity.CounterCode, entity.CounterName, entity.IsActive, terminals));
        }
    }
}

// ── DeletePosCounter ───────────────────────────────────────────────────────────
public static class DeletePosCounter
{
    public sealed record Command(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var entity = await _context.PosCounters
                .FirstOrDefaultAsync(c => c.Id == command.Id && !c.IsDeleted, ct);

            if (entity == null)
                return Result.Failure(Error.NotFound("POS counter not found."));

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
