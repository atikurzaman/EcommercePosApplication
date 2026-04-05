using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class UserLogins
{
    public Guid Id { get; set; }

    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public Guid UserId { get; set; }

    public virtual Users User { get; set; } = null!;
}
