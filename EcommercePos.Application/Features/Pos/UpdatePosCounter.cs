using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

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
