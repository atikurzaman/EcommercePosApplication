using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class UserRoles
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public virtual Users User { get; set; } = null!;
    public virtual Roles Role { get; set; } = null!;
}
