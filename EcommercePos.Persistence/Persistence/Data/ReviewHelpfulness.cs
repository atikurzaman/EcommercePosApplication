using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class ReviewHelpfulness
{
    public Guid ReviewId { get; set; }

    public Guid UserId { get; set; }

    public bool IsHelpful { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Reviews Review { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
