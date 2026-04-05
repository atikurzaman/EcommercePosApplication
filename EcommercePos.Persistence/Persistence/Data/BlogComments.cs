using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class BlogComments
{
    public Guid Id { get; set; }

    public Guid BlogId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsApproved { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Blogs Blog { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<BlogComments> InverseParentComment { get; set; } = new List<BlogComments>();

    public virtual BlogComments? ParentComment { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }
}
