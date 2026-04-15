using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ShippingMethods : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? CarrierName { get; set; }

    public decimal BaseCost { get; set; }

    public decimal CostPerKg { get; set; }

    public int EstimatedDaysMin { get; set; }

    public int EstimatedDaysMax { get; set; }

    public bool IsActive { get; set; }

    public bool IsFreeShipping { get; set; }

    public decimal? FreeShippingThreshold { get; set; }

    public int DisplayOrder { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
