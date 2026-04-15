using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ShippingCarriers
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? TrackingUrlPrefix { get; set; }

    public bool IsActive { get; set; }

    public decimal BaseCost { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
