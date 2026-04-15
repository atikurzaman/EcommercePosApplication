using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class PaymentGateways : AuditableEntity<Guid>
{
    public string MethodCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? ConfigurationJson { get; set; }

    public bool IsActive { get; set; }

    public bool IsLiveMode { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PaymentMethods MethodCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
