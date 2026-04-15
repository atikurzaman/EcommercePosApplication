using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Permissions : AuditableEntity<Guid>
{
    public string PermissionCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Module { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
