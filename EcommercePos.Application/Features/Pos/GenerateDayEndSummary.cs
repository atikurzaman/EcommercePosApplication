using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Shared.Common;

namespace EcommercePos.Application.Features.Pos;

// ── GenerateDayEndSummary ──────────────────────────────────────────────────────
public static class GenerateDayEndSummary
{
    public sealed record Command(DateOnly SummaryDate, Guid WarehouseId, Guid ClosedByUserId);

    public sealed record Response(Guid Id, DateOnly SummaryDate, string Status);

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty();
            RuleFor(x => x.ClosedByUserId).NotEmpty();
        }
    }

    public sealed class Handler
    {
        private readonly ApplicationDbContext _context;
        public Handler(ApplicationDbContext context) => _context = context;

        public async Task<Result<Response>> Handle(Command command, CancellationToken ct)
        {
            var dateStart = command.SummaryDate.ToDateTime(TimeOnly.MinValue);
            var dateEnd = command.SummaryDate.ToDateTime(TimeOnly.MaxValue);

            // Aggregate from PosTransactions for the given date/warehouse
            var transactions = await _context.PosTransactions
                .AsNoTracking()
                .Where(t => t.WarehouseId == command.WarehouseId
                    && t.SaleDate >= dateStart && t.SaleDate <= dateEnd
                    && !t.IsDeleted && t.Status != "Voided")
                .ToListAsync(ct);

            var totalSalesCount = transactions.Count;
            var totalSalesAmount = transactions.Sum(t => t.GrandTotal);
            var totalDiscount = transactions.Sum(t => t.DiscountAmount);
            var totalTax = transactions.Sum(t => t.TotalTaxAmount);
            var totalItemsSold = transactions.Sum(t => (int)t.TotalItemQuantity);
            var totalTransactions = totalSalesCount;

            // Aggregate payment tenders for breakdown
            var transactionIds = transactions.Select(t => t.Id).ToList();
            var tenders = await _context.PosPaymentTenders
                .AsNoTracking()
                .Where(p => transactionIds.Contains(p.TransactionId) && !p.IsDeleted)
                .GroupBy(p => p.MethodCode)
                .Select(g => new { MethodCode = g.Key, Total = g.Sum(p => p.Amount) })
                .ToListAsync(ct);

            var totalCashSales = tenders.Where(t => t.MethodCode == "CASH").Sum(t => t.Total);
            var totalCardSales = tenders.Where(t => t.MethodCode == "CARD").Sum(t => t.Total);
            var totalMobileSales = tenders.Where(t => t.MethodCode != "CASH" && t.MethodCode != "CARD").Sum(t => t.Total);

            // Aggregate cash shifts for opening/closing cash
            var shifts = await _context.CashShifts
                .AsNoTracking()
                .Where(s => s.WarehouseId == command.WarehouseId
                    && s.OpeningDateTime >= dateStart && s.OpeningDateTime <= dateEnd
                    && !s.IsDeleted)
                .ToListAsync(ct);

            var openingCash = shifts.Any() ? shifts.Min(s => s.OpeningCash) : 0;
            var cashInHand = shifts.Sum(s => s.ClosingCash ?? 0);
            var cashOut = 0m;

            // Aggregate expenses for the date
            var expenses = await _context.Expenses
                .AsNoTracking()
                .Where(e => e.WarehouseId == command.WarehouseId
                    && e.ExpenseDate >= dateStart && e.ExpenseDate <= dateEnd
                    && !e.IsDeleted)
                .SumAsync(e => e.Amount, ct);

            cashOut = expenses;

            var expectedCash = openingCash + totalCashSales - cashOut;
            var variance = cashInHand - expectedCash;

            // Aggregate returns
            var totalReturnAmount = await _context.PosTransactionReturns
                .AsNoTracking()
                .Where(r => r.WarehouseId == command.WarehouseId
                    && r.ReturnDate >= dateStart && r.ReturnDate <= dateEnd
                    && !r.IsDeleted)
                .SumAsync(r => r.TotalAmount, ct);

            // Customer counts
            var customerIds = transactions.Where(t => t.CustomerId.HasValue).Select(t => t.CustomerId!.Value).Distinct().ToList();
            var newCustomers = 0;
            var returningCustomers = 0;
            if (customerIds.Any())
            {
                var existingCustomers = await _context.PosTransactions
                    .AsNoTracking()
                    .Where(t => t.CustomerId.HasValue && customerIds.Contains(t.CustomerId.Value)
                        && t.SaleDate < dateStart && !t.IsDeleted)
                    .Select(t => t.CustomerId!.Value)
                    .Distinct()
                    .ToListAsync(ct);

                returningCustomers = existingCustomers.Count;
                newCustomers = customerIds.Count - returningCustomers;
            }

            // Loyalty
            var loyaltyIssued = transactions.Sum(t => t.EarnedLoyaltyPoints ?? 0);
            var loyaltyRedeemed = transactions.Sum(t => t.RedeemedLoyaltyPoints ?? 0);

            // Create or update
            var existing = await _context.DayEndSummaries
                .FirstOrDefaultAsync(d => d.SummaryDate == command.SummaryDate
                    && d.WarehouseId == command.WarehouseId && !d.IsDeleted, ct);

            if (existing == null)
            {
                existing = new DayEndSummaries
                {
                    Id = Guid.NewGuid(),
                    SummaryDate = command.SummaryDate,
                    WarehouseId = command.WarehouseId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                _context.DayEndSummaries.Add(existing);
            }

            existing.TotalSalesCount = totalSalesCount;
            existing.TotalSalesAmount = totalSalesAmount;
            existing.TotalCashSales = totalCashSales;
            existing.TotalCardSales = totalCardSales;
            existing.TotalMobileSales = totalMobileSales;
            existing.TotalReturnAmount = totalReturnAmount;
            existing.TotalDiscount = totalDiscount;
            existing.TotalTaxCollected = totalTax;
            existing.OpeningCash = openingCash;
            existing.CashInHand = cashInHand;
            existing.CashOut = cashOut;
            existing.ExpectedCash = expectedCash;
            existing.Variance = variance;
            existing.TotalItemsSold = totalItemsSold;
            existing.TotalTransactions = totalTransactions;
            existing.NewCustomers = newCustomers;
            existing.ReturningCustomers = returningCustomers;
            existing.LoyaltyPointsIssued = loyaltyIssued;
            existing.LoyaltyPointsRedeemed = loyaltyRedeemed;
            existing.Status = "Closed";
            existing.ClosedAt = DateTime.UtcNow;
            existing.ClosedByUserId = command.ClosedByUserId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Result<Response>.Success(
                new Response(existing.Id, existing.SummaryDate, existing.Status));
        }
    }
}
