using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class OrderStatuses
{
    public string StatusCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Description { get; set; }

    public byte SortOrder { get; set; }

    public bool IsTerminal { get; set; }

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
}
