using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Units : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public string? Description { get; set; }

    public Guid? BaseUnitId { get; set; }

    public decimal? ConversionFactor { get; set; }

    public bool IsActive { get; set; }
    public virtual Units? BaseUnit { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Units> InverseBaseUnit { get; set; } = new List<Units>();

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
