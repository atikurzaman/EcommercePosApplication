using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class FlashDealProducts
{
    public Guid Id { get; set; }

    public Guid FlashDealId { get; set; }

    public Guid ProductId { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal DiscountAmount { get; set; }

    public byte DiscountType { get; set; }

    public int MaxQuantity { get; set; }

    public int SoldQuantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual FlashDeals FlashDeal { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
