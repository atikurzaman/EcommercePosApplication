using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Reviews : AuditableEntity<Guid>
{
    public Guid ProductId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? OrderId { get; set; }

    public byte Rating { get; set; }

    public string? Title { get; set; }

    public string? Comment { get; set; }

    public string? MediaUrlsJson { get; set; }

    public bool IsVerifiedPurchase { get; set; }

    public bool IsApproved { get; set; }

    public bool IsFeatured { get; set; }

    public int HelpfulCount { get; set; }

    public int NotHelpfulCount { get; set; }

    public string? AdminResponse { get; set; }

    public DateTime? AdminResponseDate { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual Orders? Order { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ICollection<ReviewHelpfulness> ReviewHelpfulness { get; set; } = new List<ReviewHelpfulness>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
