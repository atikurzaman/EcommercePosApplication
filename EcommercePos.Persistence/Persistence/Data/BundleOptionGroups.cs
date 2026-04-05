using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class BundleOptionGroups
{
    public Guid Id { get; set; }

    public Guid BundleProductId { get; set; }

    public string GroupName { get; set; } = null!;

    public bool IsRequired { get; set; }

    public int MinSelections { get; set; }

    public int MaxSelections { get; set; }

    public int QuantityPerSelection { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<BundleOptionItems> BundleOptionItems { get; set; } = new List<BundleOptionItems>();

    public virtual Products BundleProduct { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<OrderBundleSelections> OrderBundleSelections { get; set; } = new List<OrderBundleSelections>();

    public virtual ICollection<PosTransactionBundleSelections> PosTransactionBundleSelections { get; set; } = new List<PosTransactionBundleSelections>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
