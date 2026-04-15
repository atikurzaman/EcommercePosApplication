using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class BlogComments : AuditableEntity<Guid>
{
    public Guid BlogId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsApproved { get; set; }
    public virtual Blogs Blog { get; set; } = null!;

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<BlogComments> InverseParentComment { get; set; } = new List<BlogComments>();

    public virtual BlogComments? ParentComment { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }
}
