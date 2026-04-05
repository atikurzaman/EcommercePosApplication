using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class CustomerTiers
{
    public string TierCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public decimal MinLifetimeSpend { get; set; }

    public decimal DiscountPct { get; set; }

    public decimal PointsMultiplier { get; set; }

    public byte SortOrder { get; set; }

    public virtual ICollection<CustomerProfiles> CustomerProfiles { get; set; } = new List<CustomerProfiles>();

    public virtual ICollection<Discounts> Discounts { get; set; } = new List<Discounts>();

    public virtual ICollection<PriceLists> PriceLists { get; set; } = new List<PriceLists>();
}
