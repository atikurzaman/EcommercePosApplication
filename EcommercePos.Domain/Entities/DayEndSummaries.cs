using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class DayEndSummaries
{
    public Guid Id { get; set; }

    public DateOnly SummaryDate { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid? CashShiftId { get; set; }

    public int TotalSalesCount { get; set; }

    public decimal TotalSalesAmount { get; set; }

    public decimal TotalCashSales { get; set; }

    public decimal TotalCardSales { get; set; }

    public decimal TotalMobileSales { get; set; }

    public decimal TotalReturnAmount { get; set; }

    public decimal TotalDiscount { get; set; }

    public decimal TotalTaxCollected { get; set; }

    public decimal OpeningCash { get; set; }

    public decimal CashInHand { get; set; }

    public decimal CashOut { get; set; }

    public decimal ExpectedCash { get; set; }

    public decimal Variance { get; set; }

    public int TotalItemsSold { get; set; }

    public int TotalTransactions { get; set; }

    public int NewCustomers { get; set; }

    public int ReturningCustomers { get; set; }

    public decimal LoyaltyPointsIssued { get; set; }

    public decimal LoyaltyPointsRedeemed { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime? ClosedAt { get; set; }

    public Guid? ClosedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual CashShifts? CashShift { get; set; }

    public virtual Users? ClosedByUser { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
