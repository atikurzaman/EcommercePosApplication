using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

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
