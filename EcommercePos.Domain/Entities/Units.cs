using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Units
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public string? Description { get; set; }

    public Guid? BaseUnitId { get; set; }

    public decimal? ConversionFactor { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Units? BaseUnit { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Units> InverseBaseUnit { get; set; } = new List<Units>();

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
