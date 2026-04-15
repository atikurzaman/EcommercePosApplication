using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Permissions
{
    public Guid Id { get; set; }

    public string PermissionCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Module { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
