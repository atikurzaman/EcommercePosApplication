using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class DeliveryZoneRegions : AuditableEntity<Guid>
{
    public Guid DeliveryZoneId { get; set; }

    public string Country { get; set; } = null!;

    public string? State { get; set; }

    public string? City { get; set; }

    public string? Area { get; set; }

    public string? PostalCode { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual DeliveryZones DeliveryZone { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
