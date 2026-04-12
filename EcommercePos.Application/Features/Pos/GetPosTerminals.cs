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
