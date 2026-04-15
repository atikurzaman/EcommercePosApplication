using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class DeliveryZoneRegions
{
    public Guid Id { get; set; }

    public Guid DeliveryZoneId { get; set; }

    public string Country { get; set; } = null!;

    public string? State { get; set; }

    public string? City { get; set; }

    public string? Area { get; set; }

    public string? PostalCode { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual DeliveryZones DeliveryZone { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
