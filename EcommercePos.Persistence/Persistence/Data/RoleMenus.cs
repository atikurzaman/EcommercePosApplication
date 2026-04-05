using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class RoleMenus
{
    public Guid Id { get; set; }

    public Guid RoleId { get; set; }

    public Guid MenuId { get; set; }

    public bool CanView { get; set; }

    public bool CanAdd { get; set; }

    public bool CanEdit { get; set; }

    public bool CanDelete { get; set; }

    public bool CanApprove { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Menus Menu { get; set; } = null!;

    public virtual Roles Role { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
