using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class PriceLists
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string PriceListType { get; set; } = null!;

    public string? TierCode { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<PriceListItems> PriceListItems { get; set; } = new List<PriceListItems>();

    public virtual CustomerTiers? TierCodeNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
