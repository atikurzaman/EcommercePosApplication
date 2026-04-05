using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class PaymentGateways
{
    public Guid Id { get; set; }

    public string MethodCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? ConfigurationJson { get; set; }

    public bool IsActive { get; set; }

    public bool IsLiveMode { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual PaymentMethods MethodCodeNavigation { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
