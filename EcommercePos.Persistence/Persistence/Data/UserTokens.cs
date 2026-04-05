using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class UserTokens
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }

    public virtual Users User { get; set; } = null!;
}
