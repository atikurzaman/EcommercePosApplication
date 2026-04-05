using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class BlogTags
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Blogs> Blog { get; set; } = new List<Blogs>();
}
