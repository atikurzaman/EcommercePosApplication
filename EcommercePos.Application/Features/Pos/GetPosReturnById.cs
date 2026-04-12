using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosReturnById ───────────────────────────────────────────────────────────
public static class GetPosReturnById
{
    public sealed record ReturnLineInfo(
        Guid Id,
        Guid ProductId,
        Guid? VariantId,
        Guid? BatchId,
        string ProductName,
        string? Sku,
        decimal Quantity,
        decimal UnitPrice,
        decimal LineTotal);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ReturnNo { get; init; } = string.Empty;
        public DateTime ReturnDate { get; init; }
        public Guid WarehouseId { get; init; }
        public string WarehouseName { get; init; } = string.Empty;
        public Guid? CustomerId { get; init; }
        public string? CustomerName { get; init; }
        public Guid? SaleId { get; init; }
        public string? SaleReceiptNumber { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Notes { get; init; }
        public Guid? CreatedByUserId { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<ReturnLineInfo> Lines { get; init; } = new();
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var ret = await _context.PosTransactionReturns
                .Include(r => r.Warehouse)
                .Include(r => r.Customer)
                .Include(r => r.Sale)
                .Include(r => r.PosTransactionReturnLines)
                    .ThenInclude(l => l.Product)
                .Where(r => r.Id == query.Id && !r.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (ret == null)
                return Result<Response>.Failure(Error.NotFound("Return not found"));

            var response = new Response
            {
                Id = ret.Id,
                ReturnNo = ret.ReturnNo,
                ReturnDate = ret.ReturnDate,
                WarehouseId = ret.WarehouseId,
                WarehouseName = ret.Warehouse?.Name ?? string.Empty,
                CustomerId = ret.CustomerId,
                CustomerName = ret.Customer != null ? (ret.Customer.CompanyName ?? ret.Customer.CustomerCode) : null,
                SaleId = ret.SaleId,
                SaleReceiptNumber = ret.Sale?.ReceiptNumber,
                TotalAmount = ret.TotalAmount,
                Notes = ret.Notes,
                CreatedByUserId = ret.CreatedByUserId,
                CreatedAt = ret.CreatedAt,
                Lines = ret.PosTransactionReturnLines
                    .Where(l => !l.IsDeleted)
                    .Select(l => new ReturnLineInfo(
                        l.Id,
                        l.ProductId,
                        l.VariantId,
                        l.BatchId,
                        l.Product?.Name ?? string.Empty,
                        l.Product?.Sku,
                        l.Quantity,
                        l.UnitPrice,
                        l.LineTotal))
                    .ToList()
            };

            return Result<Response>.Success(response);
        }
    }
}
