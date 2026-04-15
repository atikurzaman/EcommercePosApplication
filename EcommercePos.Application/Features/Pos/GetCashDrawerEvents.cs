using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetCashDrawerEvents ────────────────────────────────────────────────────────
public static class GetCashDrawerEvents
{
    public sealed record Request(Guid CashShiftId);

    public sealed record Response(
        Guid Id, string EventType, decimal Amount,
        string? Notes, DateTime OccurredAt,
        Guid PerformedBy, string? PerformedByName);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<Response>>> Handle(Request request, CancellationToken ct)
        {
            var shiftExists = await _context.CashShifts
                .AnyAsync(s => s.Id == request.CashShiftId && !s.IsDeleted, ct);
            if (!shiftExists)
                return Result<List<Response>>.Failure(Error.NotFound("Cash shift not found."));

            var items = await _context.CashDrawerEvents
                .AsNoTracking()
                .Where(e => e.CashShiftId == request.CashShiftId && !e.IsDeleted)
                .OrderBy(e => e.OccurredAt)
                .Select(e => new Response(
                    e.Id, e.EventType, e.Amount,
                    e.Notes, e.OccurredAt,
                    e.PerformedBy,
                    e.PerformedByNavigation != null ? (e.PerformedByNavigation.FirstName + " " + e.PerformedByNavigation.LastName) : null))
                .ToListAsync(ct);

            return Result<List<Response>>.Success(items);
        }
    }
}
