using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Inventory;

public static class GetStockMovements
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public Guid StockItemId { get; init; }
        public string MovementType { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
        public decimal QuantityBefore { get; init; }
        public decimal QuantityAfter { get; init; }
        public string? ReferenceNo { get; init; }
        public string? Notes { get; init; }
        public Guid? CreatedBy { get; init; }
        public string? CreatedByName { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public sealed record Query(Guid StockItemId);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Response>>> Handle(Query query, CancellationToken ct)
        {
            var movements = await _context.StockMovements
                .Include(m => m.CreatedByNavigation)
                .Where(m => m.StockItemId == query.StockItemId)
                .OrderByDescending(m => m.CreatedAt)
                .AsNoTracking()
                .Select(m => new Response
                {
                    Id = m.Id,
                    StockItemId = m.StockItemId ?? Guid.Empty,
                    MovementType = m.MovementTypeCode,
                    Quantity = m.QuantityIn,
                    QuantityBefore = m.BalanceAfter - m.QuantityIn,
                    QuantityAfter = m.BalanceAfter,
                    ReferenceNo = m.ReferenceNumber,
                    Notes = m.Notes,
                    CreatedBy = m.CreatedBy,
                    CreatedByName = m.CreatedByNavigation != null ? m.CreatedByNavigation.UserName : null,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync(ct);

            return Result<List<Response>>.Success(movements);
        }
    }
}
