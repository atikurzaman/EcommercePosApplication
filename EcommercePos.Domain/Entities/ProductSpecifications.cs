using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ProductSpecifications
{
    public Guid Id { get; set; }

    public string SpecName { get; set; } = null!;

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<ProductSpecificationValues> ProductSpecificationValues { get; set; } = new List<ProductSpecificationValues>();
}
