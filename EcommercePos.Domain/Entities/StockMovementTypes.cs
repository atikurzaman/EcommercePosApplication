using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class StockMovementTypes
{
    public string TypeCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public bool IsInbound { get; set; }

    public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();
}
