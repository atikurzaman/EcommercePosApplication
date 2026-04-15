using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class RefundRequests : AuditableEntity<Guid>
{
    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? ReturnId { get; set; }

    public decimal RefundAmount { get; set; }

    public string Reason { get; set; } = null!;

    public string StatusCode { get; set; } = null!;

    public string? AdminNote { get; set; }

    public bool ReturnToWallet { get; set; }

    public DateTime? RefundedAt { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual Orders Order { get; set; } = null!;

    public virtual Returns? Return { get; set; }

    public virtual ReturnStatuses StatusCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
