using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.StockTransfer;

public static class GetStockTransferById
{
    public sealed record Query(Guid Id);

    public sealed record LineResponse(
        Guid Id, Guid ProductId, string ProductName, Guid? VariantId, decimal Quantity);

    public sealed record Response(
        Guid Id, string TransferNo,
        Guid FromWarehouseId, string FromWarehouseName,
        Guid ToWarehouseId, string ToWarehouseName,
        DateTime TransferDate, string Status, string? Notes, DateTime CreatedAt,
        List<LineResponse> Lines);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var transfer = await _context.StockTransfers
                .Include(t => t.FromWarehouse)
                .Include(t => t.ToWarehouse)
                .Include(t => t.StockTransferLines).ThenInclude(l => l.Product)
                .Where(t => t.Id == query.Id && !t.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (transfer is null)
                return Result<Response>.Failure(Error.NotFound($"Stock transfer '{query.Id}' was not found."));

            var lines = transfer.StockTransferLines
                .Select(l => new LineResponse(l.Id, l.ProductId, l.Product.Name, l.VariantId, l.Quantity))
                .ToList();

            return Result<Response>.Success(new Response(
                transfer.Id, transfer.TransferNo,
                transfer.FromWarehouseId, transfer.FromWarehouse.Name,
                transfer.ToWarehouseId, transfer.ToWarehouse.Name,
                transfer.TransferDate, transfer.Status, transfer.Notes, transfer.CreatedAt,
                lines));
        }
    }
}
