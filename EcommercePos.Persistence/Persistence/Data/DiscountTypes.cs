using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class DiscountTypes
{
    public string TypeCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public virtual ICollection<Discounts> Discounts { get; set; } = new List<Discounts>();
}
