using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class FlashDealProducts : AuditableEntity<Guid>
{
    public Guid FlashDealId { get; set; }

    public Guid ProductId { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public byte DiscountType { get; set; }

    public int MaxQuantity { get; set; }

    public int SoldQuantity { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual FlashDeals FlashDeal { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
