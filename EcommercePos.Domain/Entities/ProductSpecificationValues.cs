using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ProductSpecificationValues : AuditableEntity<Guid>
{
    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public Guid SpecId { get; set; }

    public string Value { get; set; } = null!;
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ProductSpecifications Spec { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
