using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ProductTags
{
    public Guid ProductId { get; set; }

    public Guid TagId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual Tags Tag { get; set; } = null!;
}
