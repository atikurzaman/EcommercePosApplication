using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class PriceListItems : AuditableEntity<Guid>
{
    public Guid PriceListId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public decimal SalePrice { get; set; }

    public decimal? MinQuantity { get; set; }

    public decimal? MaxQuantity { get; set; }

    public DateTime EffectiveDate { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PriceLists PriceList { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
