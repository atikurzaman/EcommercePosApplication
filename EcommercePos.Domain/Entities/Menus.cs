using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Menus
{
    public Guid Id { get; set; }

    public Guid? ParentMenuId { get; set; }

    public string MenuCode { get; set; } = null!;

    public string MenuName { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? MenuUrl { get; set; }

    public string? IconClass { get; set; }

    public int DisplayOrder { get; set; }

    public byte MenuLevel { get; set; }

    public string? PermissionCode { get; set; }

    public bool IsActive { get; set; }

    public bool IsVisible { get; set; }

    public bool IsExternalLink { get; set; }

    public bool OpenInNewTab { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Menus> InverseParentMenu { get; set; } = new List<Menus>();

    public virtual Menus? ParentMenu { get; set; }

    public virtual ICollection<RoleMenus> RoleMenus { get; set; } = new List<RoleMenus>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
