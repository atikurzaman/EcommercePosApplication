using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ProductPriceHistories
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid ChangedByUserId { get; set; }

    public decimal OldCostPrice { get; set; }

    public decimal OldSalePrice { get; set; }

    public decimal NewCostPrice { get; set; }

    public decimal NewSalePrice { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users ChangedByUser { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;
}
