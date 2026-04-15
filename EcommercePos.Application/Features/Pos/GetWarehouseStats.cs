using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetWarehouseStats ──────────────────────────────────────────────────────────
public static class GetWarehouseStats
{
    public sealed record Query();

    public sealed record Response(int TotalCount, int ActiveCount);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var total = await _context.Warehouses
                .CountAsync(w => !w.IsDeleted, ct);
            var active = await _context.Warehouses
                .CountAsync(w => !w.IsDeleted && w.IsActive, ct);

            return Result<Response>.Success(new Response(total, active));
        }
    }
}
