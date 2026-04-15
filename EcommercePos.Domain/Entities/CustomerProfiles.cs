using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class CustomerProfiles : AuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }

    public string TierCode { get; set; } = null!;

    public bool NewsletterOptIn { get; set; }

    public bool SmsOptIn { get; set; }

    public DateTime? TierUpgradeDate { get; set; }

    public DateTime? TierReviewDate { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual CustomerTiers TierCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
