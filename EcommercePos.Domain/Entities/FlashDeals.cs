using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class FlashDeals
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public bool ShowInHomePage { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<FlashDealProducts> FlashDealProducts { get; set; } = new List<FlashDealProducts>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
