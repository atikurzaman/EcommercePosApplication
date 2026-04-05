using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class Categories
{
    public Guid Id { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<DiscountApplicability> DiscountApplicability { get; set; } = new List<DiscountApplicability>();

    public virtual ICollection<Categories> InverseParentCategory { get; set; } = new List<Categories>();

    public virtual Categories? ParentCategory { get; set; }

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual ICollection<Brands> Brand { get; set; } = new List<Brands>();
}
