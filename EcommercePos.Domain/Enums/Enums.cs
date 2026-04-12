namespace EcommercePos.Domain.Enums;

public enum CustomerType
{
    Individual = 1,
    Business = 2,
    Wholesale = 3,
    VIP = 4
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
    PreferNotToSay = 4
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Returned = 7,
    OnHold = 8
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Refunded = 5,
    PartiallyRefunded = 6,
    Cancelled = 7
}

public enum ShipmentStatus
{
    Pending = 1,
    Packed = 2,
    Shipped = 3,
    InTransit = 4,
    OutForDelivery = 5,
    Delivered = 6,
    FailedDelivery = 7,
    Returned = 8,
    Cancelled = 9
}

public enum ReturnStatus
{
    Requested = 1,
    Approved = 2,
    Rejected = 3,
    Received = 4,
    Processing = 5,
    Completed = 6,
    Cancelled = 7
}

public enum PurchaseOrderStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4,
    PartiallyReceived = 5,
    Completed = 6,
    Cancelled = 7
}

public enum StockTransferStatus
{
    Draft = 1,
    Pending = 2,
    Approved = 3,
    InTransit = 4,
    Received = 5,
    Rejected = 6,
    Cancelled = 7
}

public enum WalletTransactionStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Reversed = 4
}

public enum QuoteStatus
{
    Draft = 1,
    Sent = 2,
    Viewed = 3,
    Accepted = 4,
    Rejected = 5,
    Expired = 6,
    Converted = 7
}

public enum CashShiftStatus
{
    Open = 1,
    Suspended = 2,
    Closed = 3,
    Override = 4
}

public enum SupportTicketStatus
{
    Open = 1,
    InProgress = 2,
    WaitingCustomer = 3,
    Resolved = 4,
    Closed = 5
}

public enum LoyaltyTransactionType
{
    Earned = 1,
    Redeemed = 2,
    Bonus = 3,
    Expired = 4,
    Adjusted = 5
}

public enum InventoryAdjustmentType
{
    StockIn = 1,
    StockOut = 2,
    Damage = 3,
    Loss = 4,
    Found = 5,
    Audit = 6
}

public enum ExpenseType
{
    Operational = 1,
    Marketing = 2,
    Salaries = 3,
    Utilities = 4,
    Rent = 5,
    Maintenance = 6,
    Other = 7
}

public enum StockMovementType
{
    Purchase = 1,
    Sale = 2,
    Transfer = 3,
    Adjustment = 4,
    Return = 5,
    Damage = 6,
    Loss = 7
}
