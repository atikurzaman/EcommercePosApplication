using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class DiscountApplicability
{
    public Guid Id { get; set; }

    public Guid DiscountId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? CategoryId { get; set; }

    public virtual Categories? Category { get; set; }

    public virtual Discounts Discount { get; set; } = null!;

    public virtual Products? Product { get; set; }
}
