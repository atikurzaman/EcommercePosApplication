using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class VwCustomerLoyaltyBalance
{
    public Guid CustomerId { get; set; }

    public int? CurrentBalance { get; set; }

    public int? TotalEarned { get; set; }

    public int? TotalRedeemed { get; set; }

    public int? EarnTransactions { get; set; }

    public DateTime? NextExpiryDate { get; set; }
}
