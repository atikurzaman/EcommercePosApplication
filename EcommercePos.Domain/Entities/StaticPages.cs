using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class StaticPages : AuditableEntity<Guid>
{
    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsPublished { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
