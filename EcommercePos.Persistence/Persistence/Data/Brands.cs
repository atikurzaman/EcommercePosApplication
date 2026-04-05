using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class Brands
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? LogoUrl { get; set; }

    public string? Website { get; set; }

    public string? CountryOfOrigin { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<Categories> Category { get; set; } = new List<Categories>();
}
