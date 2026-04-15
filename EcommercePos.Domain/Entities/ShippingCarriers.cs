using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ShippingCarriers : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? TrackingUrlPrefix { get; set; }

    public bool IsActive { get; set; }

    public decimal BaseCost { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
