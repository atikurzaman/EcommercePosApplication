using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class RoleClaims
{
    public Guid Id { get; set; }

    public Guid RoleId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual Roles Role { get; set; } = null!;
}
