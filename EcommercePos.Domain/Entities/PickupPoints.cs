using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class PickupPoints
{
    public Guid Id { get; set; }

    public Guid? WarehouseId { get; set; }

    public string Name { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? PostalCode { get; set; }

    public string Phone { get; set; } = null!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public TimeOnly? OpeningTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses? Warehouse { get; set; }
}
