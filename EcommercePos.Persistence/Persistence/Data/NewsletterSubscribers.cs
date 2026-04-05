using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class NewsletterSubscribers
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public Guid? CustomerId { get; set; }

    public bool IsActive { get; set; }

    public DateTime SubscribedAt { get; set; }

    public DateTime? UnsubscribedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Customers? Customer { get; set; }
}
