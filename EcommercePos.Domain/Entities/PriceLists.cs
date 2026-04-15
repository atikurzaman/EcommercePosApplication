using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class PriceLists : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string PriceListType { get; set; } = null!;

    public string? TierCode { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public string? Description { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<PriceListItems> PriceListItems { get; set; } = new List<PriceListItems>();

    public virtual CustomerTiers? TierCodeNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
