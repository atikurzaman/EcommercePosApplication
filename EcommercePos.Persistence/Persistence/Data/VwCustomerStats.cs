using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class VwCustomerStats
{
    public Guid CustomerId { get; set; }

    public int? OrderCount { get; set; }

    public decimal TotalSpent { get; set; }

    public DateTime? LastOrderDate { get; set; }

    public int? CompletedOrders { get; set; }

    public decimal TotalRefunded { get; set; }
}
