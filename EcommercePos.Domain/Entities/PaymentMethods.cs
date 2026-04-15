using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PaymentMethods
{
    public string MethodCode { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public bool IsOnline { get; set; }

    public bool IsActive { get; set; }

    public byte SortOrder { get; set; }

    public virtual ICollection<Expenses> Expenses { get; set; } = new List<Expenses>();

    public virtual ICollection<PaymentGateways> PaymentGateways { get; set; } = new List<PaymentGateways>();

    public virtual ICollection<Payments> Payments { get; set; } = new List<Payments>();

    public virtual ICollection<PosPaymentTenders> PosPaymentTenders { get; set; } = new List<PosPaymentTenders>();

    public virtual ICollection<Returns> Returns { get; set; } = new List<Returns>();
}
