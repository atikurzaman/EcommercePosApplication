using Microsoft.EntityFrameworkCore;
using Mapster;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.PosTransaction;

public static class GetPosTransactions
{
    public sealed record Request(int PageIndex = 0, int PageSize = 10, string? Search = null, Guid? CashierId = null, string? Status = null, DateTime? StartDate = null, DateTime? EndDate = null);

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ReceiptNumber { get; init; } = string.Empty;
        public DateTime SaleDate { get; init; }
        public string SaleType { get; init; } = string.Empty;
        public decimal SubTotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal GrandTotal { get; init; }
        public decimal PaidAmount { get; init; }
        public int TotalItemQuantity { get; init; }
        public string Status { get; init; } = string.Empty;
        public string? CashierName { get; init; }
        public string? CustomerName { get; init; }
    }

    public sealed record Query(int PageIndex, int PageSize, string? Search, Guid? CashierId, string? Status, DateTime? StartDate, DateTime? EndDate);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Response>>> Handle(Query query, CancellationToken ct)
        {
            var dbQuery = _context.PosTransactions
                .Where(t => !t.IsDeleted)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                dbQuery = dbQuery.Where(t => t.ReceiptNumber.Contains(query.Search) || t.CustomerName.Contains(query.Search));
            }

            if (query.CashierId.HasValue)
            {
                dbQuery = dbQuery.Where(t => t.CashierId == query.CashierId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                dbQuery = dbQuery.Where(t => t.Status == query.Status);
            }

            if (query.StartDate.HasValue)
            {
                dbQuery = dbQuery.Where(t => t.SaleDate >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                dbQuery = dbQuery.Where(t => t.SaleDate <= query.EndDate.Value);
            }

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .Include(t => t.Cashier)
                .OrderByDescending(t => t.SaleDate)
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .ProjectToType<Response>()
                .ToListAsync(ct);

            return Result<PagedResult<Response>>.Success(new PagedResult<Response>(items, totalCount, query.PageIndex, query.PageSize));
        }
    }
}

public static class GetPosTransactionById
{
    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ReceiptNumber { get; init; } = string.Empty;
        public DateTime SaleDate { get; init; }
        public string SaleType { get; init; } = string.Empty;
        public decimal SubTotal { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal TotalTaxAmount { get; init; }
        public decimal RoundOffAmount { get; init; }
        public decimal GrandTotal { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal ChangeAmount { get; init; }
        public int TotalItemQuantity { get; init; }
        public string Status { get; init; } = string.Empty;
        public string? VoidReason { get; init; }
        public string? Notes { get; init; }
        public string? CashierName { get; init; }
        public string? CustomerName { get; init; }
        public string? CustomerPhone { get; init; }
        public List<TransactionLineResponse> Lines { get; init; } = new();
        public List<PaymentTenderResponse> Payments { get; init; } = new();
    }

    public sealed record TransactionLineResponse
    {
        public Guid Id { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public decimal Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal TaxAmount { get; init; }
        public decimal LineTotal { get; init; }
    }

    public sealed record PaymentTenderResponse
    {
        public Guid Id { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string? CardLastFour { get; init; }
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
            var transaction = await _context.PosTransactions
                .Include(t => t.Cashier)
                .Include(t => t.PosTransactionLines)
                .Include(t => t.PosPaymentTenders)
                .Where(t => t.Id == query.Id && !t.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (transaction == null)
            {
                return Result<Response>.Failure(Error.NotFound("Transaction not found"));
            }

            var response = new Response
            {
                Id = transaction.Id,
                ReceiptNumber = transaction.ReceiptNumber,
                SaleDate = transaction.SaleDate,
                SaleType = transaction.SaleType,
                SubTotal = transaction.SubTotal,
                DiscountAmount = transaction.DiscountAmount,
                TotalTaxAmount = transaction.TotalTaxAmount,
                RoundOffAmount = transaction.RoundOffAmount,
                GrandTotal = transaction.GrandTotal,
                PaidAmount = transaction.PaidAmount,
                ChangeAmount = transaction.ChangeAmount,
                TotalItemQuantity = (int)transaction.TotalItemQuantity,
                Status = transaction.Status,
                VoidReason = transaction.VoidReason,
                Notes = transaction.Notes,
                CashierName = transaction.Cashier?.UserName,
                CustomerName = transaction.CustomerName,
                CustomerPhone = transaction.CustomerPhone,
                Lines = transaction.PosTransactionLines.Where(l => !l.IsDeleted).Select(l => new TransactionLineResponse
                {
                    Id = l.Id,
                    ProductName = l.ProductName,
                    Sku = l.Sku,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    DiscountAmount = l.DiscountAmount,
                    TaxAmount = l.TaxAmount,
                    LineTotal = l.LineTotal
                }).ToList(),
                Payments = transaction.PosPaymentTenders.Select(p => new PaymentTenderResponse
                {
                    Id = p.Id,
                    PaymentMethod = p.MethodCode,
                    Amount = p.Amount,
                    CardLastFour = p.CardLast4
                }).ToList()
            };

            return Result<Response>.Success(response);
        }
    }
}

public static class CreatePosTransaction
{
    public sealed record Request
    {
        public Guid CashShiftId { get; init; }
        public Guid PosCounterId { get; init; }
        public Guid? CustomerId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerPhone { get; init; } = string.Empty;
        public string SaleType { get; init; } = "REGULAR";
        public List<TransactionLineRequest> Lines { get; init; } = new();
        public List<PaymentRequest> Payments { get; init; } = new();
        public string? Notes { get; init; }
    }

    public sealed record TransactionLineRequest
    {
        public Guid ProductId { get; init; }
        public Guid? VariantId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public decimal Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal DiscountPercent { get; init; }
        public decimal DiscountAmount { get; init; }
        public decimal TaxAmount { get; init; }
        public decimal LineTotal { get; init; }
    }

    public sealed record PaymentRequest
    {
        public string PaymentMethod { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string? CardLastFour { get; init; }
    }

    public sealed record Response
    {
        public Guid Id { get; init; }
        public string ReceiptNumber { get; init; } = string.Empty;
        public decimal GrandTotal { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal ChangeAmount { get; init; }
    }

    public sealed record Command(Request Request, Guid CashierId, Guid WarehouseId);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var shift = await _context.CashShifts.FindAsync(new object[] { command.Request.CashShiftId }, ct);
            if (shift == null)
                return Result<Response>.Failure(Error.NotFound("Cash shift not found"));

            var counter = await _context.PosCounters.FindAsync(new object[] { command.Request.PosCounterId }, ct);
            if (counter == null)
                return Result<Response>.Failure(Error.NotFound("POS counter not found"));

            var receiptNumber = $"RC{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString()[..6].ToUpper()}";
            var subTotal = command.Request.Lines.Sum(l => l.LineTotal);
            var discountAmount = command.Request.Lines.Sum(l => l.DiscountAmount);
            var taxAmount = command.Request.Lines.Sum(l => l.TaxAmount);
            var grandTotal = subTotal + taxAmount - discountAmount;
            var paidAmount = command.Request.Payments.Sum(p => p.Amount);
            var totalQty = command.Request.Lines.Sum(l => l.Quantity);

            var transaction = new PosTransactions
            {
                Id = Guid.NewGuid(),
                ReceiptNumber = receiptNumber,
                CashShiftId = command.Request.CashShiftId,
                PosCounterId = command.Request.PosCounterId,
                CashierId = command.CashierId,
                CustomerId = command.Request.CustomerId,
                WarehouseId = command.WarehouseId,
                SaleDate = DateTime.Now,
                SaleType = command.Request.SaleType,
                SubTotal = subTotal,
                DiscountAmount = discountAmount,
                TotalTaxAmount = taxAmount,
                GrandTotal = grandTotal,
                PaidAmount = paidAmount,
                ChangeAmount = paidAmount - grandTotal,
                TotalItemQuantity = totalQty,
                Status = "COMPLETED",
                CustomerName = command.Request.CustomerName,
                CustomerPhone = command.Request.CustomerPhone,
                Notes = command.Request.Notes,
                CreatedAt = DateTime.Now,
                CreatedBy = command.CashierId
            };

            foreach (var line in command.Request.Lines)
            {
                var transactionLine = new PosTransactionLines
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transaction.Id,
                    ProductId = line.ProductId,
                    VariantId = line.VariantId,
                    ProductName = line.ProductName,
                    Sku = line.Sku,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    DiscountPercent = line.DiscountPercent,
                    DiscountAmount = line.DiscountAmount,
                    TaxAmount = line.TaxAmount,
                    LineTotal = line.LineTotal,
                    CreatedAt = DateTime.Now,
                    CreatedBy = command.CashierId,
                    IsDeleted = false
                };
                transaction.PosTransactionLines.Add(transactionLine);

                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == line.ProductId && s.WarehouseId == command.WarehouseId, ct);
                if (stockItem != null)
                {
                    stockItem.QuantityOnHand -= line.Quantity;
                }
            }

            foreach (var payment in command.Request.Payments)
            {
                var paymentTender = new PosPaymentTenders
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transaction.Id,
                    MethodCode = payment.PaymentMethod,
                    Amount = payment.Amount,
                    CardLast4 = payment.CardLastFour,
                    PaymentDate = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
                transaction.PosPaymentTenders.Add(paymentTender);
            }

            _context.PosTransactions.Add(transaction);
            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(new Response
            {
                Id = transaction.Id,
                ReceiptNumber = transaction.ReceiptNumber,
                GrandTotal = transaction.GrandTotal,
                PaidAmount = transaction.PaidAmount,
                ChangeAmount = transaction.ChangeAmount
            });
        }
    }
}

public static class VoidPosTransaction
{
    public sealed record Request(Guid Id, Guid VoidedBy, string Reason);

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Request request, CancellationToken ct)
        {
            var transaction = await _context.PosTransactions
                .Include(t => t.PosTransactionLines)
                .Where(t => t.Id == request.Id && !t.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (transaction == null)
                return Result.Failure(Error.NotFound("Transaction not found"));

            if (transaction.Status == "VOIDED")
                return Result.Failure(Error.Conflict("Transaction already voided"));

            transaction.Status = "VOIDED";
            transaction.VoidReason = request.Reason;
            transaction.VoidedBy = request.VoidedBy;
            transaction.VoidedAt = DateTime.Now;
            transaction.UpdatedAt = DateTime.Now;

            foreach (var line in transaction.PosTransactionLines.Where(l => !l.IsDeleted))
            {
                var stockItem = await _context.StockItems
                    .FirstOrDefaultAsync(s => s.ProductId == line.ProductId && s.WarehouseId == transaction.WarehouseId, ct);
                if (stockItem != null)
                {
                    stockItem.QuantityOnHand += line.Quantity;
                }
            }

            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
