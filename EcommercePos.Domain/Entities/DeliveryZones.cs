using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class DeliveryZones : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public decimal BaseDeliveryCost { get; set; }

    public decimal? FreeDeliveryThreshold { get; set; }

    public int? MinDeliveryDays { get; set; }

    public int? MaxDeliveryDays { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<DeliveryZoneRegions> DeliveryZoneRegions { get; set; } = new List<DeliveryZoneRegions>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
