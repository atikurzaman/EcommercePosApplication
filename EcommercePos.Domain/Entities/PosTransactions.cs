using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PosTransactions
{
    public Guid Id { get; set; }

    public string ReceiptNumber { get; set; } = null!;

    public Guid CashShiftId { get; set; }

    public Guid PosCounterId { get; set; }

    public Guid? PosTerminalId { get; set; }

    public Guid CashierId { get; set; }

    public Guid? CashierEmployeeId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid? AppliedDiscountId { get; set; }

    public DateTime SaleDate { get; set; }

    public string SaleType { get; set; } = null!;

    public int? FloorTableId { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public decimal RoundOffAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal ChangeAmount { get; set; }

    public decimal TotalItemQuantity { get; set; }

    public int? EarnedLoyaltyPoints { get; set; }

    public int? RedeemedLoyaltyPoints { get; set; }

    public string? CouponCode { get; set; }

    public decimal? CouponDiscount { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }

    public string Status { get; set; } = null!;

    public string? VoidReason { get; set; }

    public Guid? VoidedBy { get; set; }

    public DateTime? VoidedAt { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Discounts? AppliedDiscount { get; set; }

    public virtual ICollection<CashDrawerEvents> CashDrawerEvents { get; set; } = new List<CashDrawerEvents>();

    public virtual CashShifts CashShift { get; set; } = null!;

    public virtual Users Cashier { get; set; } = null!;

    public virtual Employees? CashierEmployee { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers? Customer { get; set; }

    public virtual ICollection<DiscountUsageLog> DiscountUsageLog { get; set; } = new List<DiscountUsageLog>();

    public virtual ICollection<LoyaltyTransactions> LoyaltyTransactions { get; set; } = new List<LoyaltyTransactions>();

    public virtual PosCounters PosCounter { get; set; } = null!;

    public virtual ICollection<PosPaymentTenders> PosPaymentTenders { get; set; } = new List<PosPaymentTenders>();

    public virtual PosTerminals? PosTerminal { get; set; }

    public virtual ICollection<PosTransactionLines> PosTransactionLines { get; set; } = new List<PosTransactionLines>();

    public virtual ICollection<PosTransactionReturns> PosTransactionReturns { get; set; } = new List<PosTransactionReturns>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? VoidedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
