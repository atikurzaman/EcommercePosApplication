using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class OrderReturnItems : AuditableEntity<Guid>
{
    public Guid ReturnId { get; set; }

    public Guid OrderItemId { get; set; }

    public decimal Quantity { get; set; }

    public decimal RefundAmount { get; set; }

    public string? Reason { get; set; }

    public string Condition { get; set; } = null!;
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual OrderItems OrderItem { get; set; } = null!;

    public virtual Returns Return { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
