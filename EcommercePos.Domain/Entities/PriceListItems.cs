using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PriceListItems
{
    public Guid Id { get; set; }

    public Guid PriceListId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public decimal SalePrice { get; set; }

    public decimal? MinQuantity { get; set; }

    public decimal? MaxQuantity { get; set; }

    public DateTime EffectiveDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PriceLists PriceList { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
