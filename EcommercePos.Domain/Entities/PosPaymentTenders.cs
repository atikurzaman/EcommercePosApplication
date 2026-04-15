using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PosPaymentTenders
{
    public Guid Id { get; set; }

    public Guid TransactionId { get; set; }

    public string MethodCode { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? TransactionNo { get; set; }

    public string? CardLast4 { get; set; }

    public DateTime PaymentDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PaymentMethods MethodCodeNavigation { get; set; } = null!;

    public virtual PosTransactions Transaction { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
