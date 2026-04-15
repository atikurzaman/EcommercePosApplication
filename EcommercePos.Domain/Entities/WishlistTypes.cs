using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class WishlistTypes
{
    public string TypeCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public virtual ICollection<Wishlists> Wishlists { get; set; } = new List<Wishlists>();
}
