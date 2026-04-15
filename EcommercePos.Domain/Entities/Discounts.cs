using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Discounts : AuditableEntity<Guid>
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string DiscountTypeCode { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public decimal? MinimumOrderAmount { get; set; }

    public decimal? MaximumDiscountAmount { get; set; }

    public int? MaximumUsageCount { get; set; }

    public int? MaximumUsagePerUser { get; set; }

    public int CurrentUsageCount { get; set; }

    public string AppliesTo { get; set; } = null!;

    public string? TierCode { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsFirstOrderOnly { get; set; }

    public bool IsSingleUsePerCustomer { get; set; }

    public bool AutoApply { get; set; }

    public int? RequiresMinQty { get; set; }

    public bool RequiresShipping { get; set; }
    public virtual ICollection<Carts> Carts { get; set; } = new List<Carts>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<DiscountApplicability> DiscountApplicability { get; set; } = new List<DiscountApplicability>();

    public virtual DiscountTypes DiscountTypeCodeNavigation { get; set; } = null!;

    public virtual ICollection<DiscountUsageLog> DiscountUsageLog { get; set; } = new List<DiscountUsageLog>();

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

    public virtual ICollection<PosTransactions> PosTransactions { get; set; } = new List<PosTransactions>();

    public virtual CustomerTiers? TierCodeNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
