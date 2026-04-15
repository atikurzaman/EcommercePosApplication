using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class FlashDeals : AuditableEntity<Guid>
{
    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public bool ShowInHomePage { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<FlashDealProducts> FlashDealProducts { get; set; } = new List<FlashDealProducts>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
