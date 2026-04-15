using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class RefundRequests
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? ReturnId { get; set; }

    public decimal RefundAmount { get; set; }

    public string Reason { get; set; } = null!;

    public string StatusCode { get; set; } = null!;

    public string? AdminNote { get; set; }

    public bool ReturnToWallet { get; set; }

    public DateTime? RefundedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual Orders Order { get; set; } = null!;

    public virtual Returns? Return { get; set; }

    public virtual ReturnStatuses StatusCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
