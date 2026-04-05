using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class AuditLogs
{
    public long Id { get; set; }

    public Guid? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public string? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Details { get; set; }

    public DateTime OccurredAt { get; set; }

    public virtual Users? User { get; set; }
}
