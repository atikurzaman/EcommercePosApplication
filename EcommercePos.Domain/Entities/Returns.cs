using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Returns : AuditableEntity<Guid>
{
    public Guid OrderId { get; set; }

    public string ReturnNumber { get; set; } = null!;

    public string? RmaNumber { get; set; }

    public Guid? ProcessedByUserId { get; set; }

    public string Reason { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime RequestDate { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public DateTime? RefundDate { get; set; }

    public string StatusCode { get; set; } = null!;

    public decimal RefundAmount { get; set; }

    public string? RefundMethodCode { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Orders Order { get; set; } = null!;

    public virtual ICollection<OrderReturnItems> OrderReturnItems { get; set; } = new List<OrderReturnItems>();

    public virtual Users? ProcessedByUser { get; set; }

    public virtual PaymentMethods? RefundMethodCodeNavigation { get; set; }

    public virtual ICollection<RefundRequests> RefundRequests { get; set; } = new List<RefundRequests>();

    public virtual ReturnStatuses StatusCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
