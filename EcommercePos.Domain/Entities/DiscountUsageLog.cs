using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class DiscountUsageLog
{
    public Guid Id { get; set; }

    public Guid DiscountId { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? PosTransactionId { get; set; }

    public Guid UserId { get; set; }

    public Guid? CustomerId { get; set; }

    public decimal DiscountAmount { get; set; }

    public DateTime UsedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers? Customer { get; set; }

    public virtual Discounts Discount { get; set; } = null!;

    public virtual Orders? Order { get; set; }

    public virtual PosTransactions? PosTransaction { get; set; }

    public virtual Users User { get; set; } = null!;
}
