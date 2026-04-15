using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class RoleMenus : BaseEntity<Guid>
{
    public Guid RoleId { get; set; }

    public Guid MenuId { get; set; }

    public bool CanView { get; set; }

    public bool CanAdd { get; set; }

    public bool CanEdit { get; set; }

    public bool CanDelete { get; set; }

    public bool CanApprove { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Menus Menu { get; set; } = null!;

    public virtual Roles Role { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
