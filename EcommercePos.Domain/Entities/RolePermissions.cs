using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class RolePermissions
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public bool IsGranted { get; set; }

    public virtual Permissions Permission { get; set; } = null!;

    public virtual Roles Role { get; set; } = null!;
}
