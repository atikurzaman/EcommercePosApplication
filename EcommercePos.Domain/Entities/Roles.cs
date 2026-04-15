using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Roles
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<RoleClaims> RoleClaims { get; set; } = new List<RoleClaims>();

    public virtual ICollection<RoleMenus> RoleMenus { get; set; } = new List<RoleMenus>();

    public virtual ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();

    public virtual ICollection<Users> User { get; set; } = new List<Users>();
}
