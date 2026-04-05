using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class ShippingMethods
{
    public Guid Id { get; set; }

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
