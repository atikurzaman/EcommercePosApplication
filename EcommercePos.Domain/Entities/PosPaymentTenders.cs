using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class PosPaymentTenders : AuditableEntity<Guid>
{
    public Guid TransactionId { get; set; }

    public string MethodCode { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? TransactionNo { get; set; }

    public string? CardLast4 { get; set; }

    public DateTime PaymentDate { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PaymentMethods MethodCodeNavigation { get; set; } = null!;

    public virtual PosTransactions Transaction { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
