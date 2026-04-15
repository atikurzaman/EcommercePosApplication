using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Returns
{
    public Guid Id { get; set; }

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

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Orders Order { get; set; } = null!;

    public virtual ICollection<OrderReturnItems> OrderReturnItems { get; set; } = new List<OrderReturnItems>();

    public virtual Users? ProcessedByUser { get; set; }

    public virtual PaymentMethods? RefundMethodCodeNavigation { get; set; }

    public virtual ICollection<RefundRequests> RefundRequests { get; set; } = new List<RefundRequests>();

    public virtual ReturnStatuses StatusCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
