using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetActiveShift ─────────────────────────────────────────────────────────────
public static class GetActiveShift
{
    public sealed record Query(Guid? UserId, Guid? WarehouseId);

    public sealed record Response(
        Guid Id, Guid WarehouseId, string WarehouseName,
        Guid PosCounterId, string CounterName,
        Guid? PosTerminalId, string? TerminalName,
        Guid? OpenedByUserId, string? OpenedByName,
        Guid? OpenedByEmployeeId,
        string Status, DateTime OpeningDateTime,
        decimal OpeningCash, decimal TotalSalesAmount, int TotalTransactions,
        string? Notes);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var q = _context.CashShifts
                .AsNoTracking()
                .Where(s => !s.IsDeleted && s.Status == "Open");

            if (query.UserId.HasValue)
                q = q.Where(s => s.OpenedByUserId == query.UserId.Value);
            if (query.WarehouseId.HasValue)
                q = q.Where(s => s.WarehouseId == query.WarehouseId.Value);

            var entity = await q
                .Select(s => new Response(
                    s.Id, s.WarehouseId, s.Warehouse.Name,
                    s.PosCounterId, s.PosCounter.CounterName,
                    s.PosTerminalId, s.PosTerminal != null ? s.PosTerminal.TerminalName : null,
                    s.OpenedByUserId, s.OpenedByUser != null ? (s.OpenedByUser.FirstName + " " + s.OpenedByUser.LastName) : null,
                    s.OpenedByEmployeeId,
                    s.Status, s.OpeningDateTime,
                    s.OpeningCash, s.TotalSalesAmount, s.TotalTransactions,
                    s.Notes))
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                return Result<Response>.Failure(Error.NotFound("No active shift found."));

            return Result<Response>.Success(entity);
        }
    }
}
