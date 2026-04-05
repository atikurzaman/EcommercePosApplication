using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class Reviews
{
    public Guid Id { get; set; }

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

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual Orders? Order { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ICollection<ReviewHelpfulness> ReviewHelpfulness { get; set; } = new List<ReviewHelpfulness>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
