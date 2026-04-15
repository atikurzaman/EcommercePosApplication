using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ReturnStatuses
{
    public string StatusCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public byte SortOrder { get; set; }

    public virtual ICollection<RefundRequests> RefundRequests { get; set; } = new List<RefundRequests>();

    public virtual ICollection<Returns> Returns { get; set; } = new List<Returns>();
}
