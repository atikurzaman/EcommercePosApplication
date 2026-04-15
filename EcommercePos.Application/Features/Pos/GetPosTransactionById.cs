using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;
using EcommercePos.Application.Common;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GetPosTransactionById ──────────────────────────────────────────────────────
public static class GetPosTransactionById
{
    public sealed record TransactionLineInfo(
        Guid Id,
        Guid ProductId,
        Guid? VariantId,
        string ProductName,
        string? Sku,
        decimal Quantity,
        decimal UnitPrice,
        decimal DiscountPercent,
        decimal DiscountAmount,
        decimal TaxAmount,
        decimal LineTotal);

    public sealed record PaymentTenderInfo(
        Guid Id,
        string MethodCode,
        decimal Amount,
        string? TransactionNo,
        string? CardLast4,
        DateTime PaymentDate);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ReceiptNumber { get; init; } = string.Empty;
        public DateTime SaleDate { get; init; }
        public string SaleType { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid CashShiftId { get; init; }
        public Guid PosCounterId { get; init; }
        public Guid? PosTerminalId { get; init; }
        public Guid CashierId { get; init; }
        public string CashierName { get; init; } = string.Empty;
        public Guid? CustomerId { get; init; }
        public string? CustomerName { get; init; }
        public string? CustomerPhone { get; init; }
        public Guid WarehouseId { get; init; }
        public string WarehouseName { get; init; } = string.Empty;
        public string? CouponCode { get; init; }
        public decimal SubTotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal TotalTaxAmount { get; init; }
        public decimal RoundOffAmount { get; init; }
        public decimal GrandTotal { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal ChangeAmount { get; init; }
        public decimal TotalItemQuantity { get; init; }
        public string? VoidReason { get; init; }
        public Guid? VoidedBy { get; init; }
        public DateTime? VoidedAt { get; init; }
        public string? Notes { get; init; }
        public List<TransactionLineInfo> Lines { get; init; } = new();
        public List<PaymentTenderInfo> Payments { get; init; } = new();
    }

    public sealed record Query(Guid Id);

    public sealed class Handler
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Query query, CancellationToken ct)
        {
            var transaction = await _context.PosTransactions
                .Include(t => t.Cashier)
                .Include(t => t.Warehouse)
                .Include(t => t.PosTransactionLines)
                .Include(t => t.PosPaymentTenders)
                .Where(t => t.Id == query.Id && !t.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (transaction == null)
                return Result<Response>.Failure(Error.NotFound("Transaction not found"));

            var response = new Response
            {
                Id = transaction.Id,
                ReceiptNumber = transaction.ReceiptNumber,
                SaleDate = transaction.SaleDate,
                SaleType = transaction.SaleType,
                Status = transaction.Status,
                CashShiftId = transaction.CashShiftId,
                PosCounterId = transaction.PosCounterId,
                PosTerminalId = transaction.PosTerminalId,
                CashierId = transaction.CashierId,
                CashierName = transaction.Cashier?.UserName ?? string.Empty,
                CustomerId = transaction.CustomerId,
                CustomerName = transaction.CustomerName,
                CustomerPhone = transaction.CustomerPhone,
                WarehouseId = transaction.WarehouseId,
                WarehouseName = transaction.Warehouse?.Name ?? string.Empty,
                CouponCode = transaction.CouponCode,
                SubTotal = transaction.SubTotal,
                DiscountAmount = transaction.DiscountAmount,
                TotalTaxAmount = transaction.TotalTaxAmount,
                RoundOffAmount = transaction.RoundOffAmount,
                GrandTotal = transaction.GrandTotal,
                PaidAmount = transaction.PaidAmount,
                ChangeAmount = transaction.ChangeAmount,
                TotalItemQuantity = transaction.TotalItemQuantity,
                VoidReason = transaction.VoidReason,
                VoidedBy = transaction.VoidedBy,
                VoidedAt = transaction.VoidedAt,
                Notes = transaction.Notes,
                Lines = transaction.PosTransactionLines
                    .Where(l => !l.IsDeleted)
                    .Select(l => new TransactionLineInfo(
                        l.Id,
                        l.ProductId,
                        l.VariantId,
                        l.ProductName,
                        l.Sku,
                        l.Quantity,
                        l.UnitPrice,
                        l.DiscountPercent,
                        l.DiscountAmount,
                        l.TaxAmount,
                        l.LineTotal))
                    .ToList(),
                Payments = transaction.PosPaymentTenders
                    .Where(p => !p.IsDeleted)
                    .Select(p => new PaymentTenderInfo(
                        p.Id,
                        p.MethodCode,
                        p.Amount,
                        p.TransactionNo,
                        p.CardLast4,
                        p.PaymentDate))
                    .ToList()
            };

            return Result<Response>.Success(response);
        }
    }
}
