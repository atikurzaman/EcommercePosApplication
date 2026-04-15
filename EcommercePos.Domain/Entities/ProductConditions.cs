using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ProductConditions
{
    public string ConditionCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();
}
