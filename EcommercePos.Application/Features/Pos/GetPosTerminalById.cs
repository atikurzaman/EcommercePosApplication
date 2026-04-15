using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

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
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

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
