using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class OrderReturnItems
{
    public Guid Id { get; set; }

    public Guid ReturnId { get; set; }

    public Guid OrderItemId { get; set; }

    public decimal Quantity { get; set; }

    public decimal RefundAmount { get; set; }

    public string? Reason { get; set; }

    public string Condition { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual OrderItems OrderItem { get; set; } = null!;

    public virtual Returns Return { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
