using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Carts : AuditableEntity<Guid>
{
    public Guid? CustomerId { get; set; }

    public Guid? UserId { get; set; }

    public string? SessionId { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal Total { get; set; }

    public Guid? AppliedDiscountId { get; set; }

    public string? CouponCode { get; set; }

    public DateTime? ExpiresAt { get; set; }
    public virtual Discounts? AppliedDiscount { get; set; }

    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers? Customer { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }
}
