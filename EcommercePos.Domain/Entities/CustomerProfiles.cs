using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class CustomerProfiles
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string TierCode { get; set; } = null!;

    public bool NewsletterOptIn { get; set; }

    public bool SmsOptIn { get; set; }

    public DateTime? TierUpgradeDate { get; set; }

    public DateTime? TierReviewDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual CustomerTiers TierCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
