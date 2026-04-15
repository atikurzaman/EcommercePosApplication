using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class DeliveryZones
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public decimal BaseDeliveryCost { get; set; }

    public decimal? FreeDeliveryThreshold { get; set; }

    public int? MinDeliveryDays { get; set; }

    public int? MaxDeliveryDays { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<DeliveryZoneRegions> DeliveryZoneRegions { get; set; } = new List<DeliveryZoneRegions>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
