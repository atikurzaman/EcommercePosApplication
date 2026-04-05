using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class Carts
{
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? UserId { get; set; }

    public string? SessionId { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal Total { get; set; }

    public Guid? AppliedDiscountId { get; set; }

    public string? CouponCode { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Discounts? AppliedDiscount { get; set; }

    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers? Customer { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }
}
