using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class PaymentStatuses
{
    public string StatusCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();
}
