using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class UserClaims
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual Users User { get; set; } = null!;
}
