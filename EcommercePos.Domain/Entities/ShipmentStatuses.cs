using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ShipmentStatuses
{
    public string StatusCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public byte SortOrder { get; set; }

    public virtual ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();
}
