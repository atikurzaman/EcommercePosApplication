using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.InventoryAdjustment;

public static class GetInventoryAdjustmentById
{
    public sealed record Query(Guid Id);

    public sealed record LineResponse(
        Guid Id, Guid ProductId, string ProductName, Guid? VariantId,
        decimal QuantityAdjusted, string Reason);

    public sealed record Response(
        Guid Id, string AdjustmentNo, Guid WarehouseId, string WarehouseName,
        DateTime AdjustmentDate, string AdjustmentType, string Reason, string? Notes,
        bool IsApproved, DateTime? ApprovedAt, DateTime CreatedAt,
        List<LineResponse> Lines);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var adj = await _context.InventoryAdjustments
                .Include(a => a.Warehouse)
                .Include(a => a.InventoryAdjustmentLines).ThenInclude(l => l.Product)
                .Where(a => a.Id == query.Id && !a.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (adj is null)
                return Result<Response>.Failure(Error.NotFound($"Inventory adjustment '{query.Id}' was not found."));

            var lines = adj.InventoryAdjustmentLines
                .Select(l => new LineResponse(
                    l.Id, l.ProductId, l.Product.Name, l.VariantId,
                    l.AdjustmentQuantity, l.Remarks ?? string.Empty))
                .ToList();

            return Result<Response>.Success(new Response(
                adj.Id, adj.AdjustmentNo, adj.WarehouseId, adj.Warehouse.Name,
                adj.AdjustmentDate, adj.AdjustmentType, adj.Reason, adj.Notes,
                adj.ApprovedByUserId != null, adj.ApprovedAt, adj.CreatedAt,
                lines));
        }
    }
}
