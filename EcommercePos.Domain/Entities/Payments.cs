using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Payments : AuditableEntity<Guid>
{
    public Guid OrderId { get; set; }

    public string MethodCode { get; set; } = null!;

    public string? Provider { get; set; }

    public string StatusCode { get; set; } = null!;

    public decimal Amount { get; set; }

    public decimal TransactionAmount { get; set; }

    public decimal? GatewayFee { get; set; }

    public decimal RefundedAmount { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string? TransactionId { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? GatewayResponse { get; set; }

    public string? FailureReason { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? RefundedAt { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Currencies CurrencyCodeNavigation { get; set; } = null!;

    public virtual PaymentMethods MethodCodeNavigation { get; set; } = null!;

    public virtual Orders Order { get; set; } = null!;

    public virtual PaymentStatuses StatusCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
