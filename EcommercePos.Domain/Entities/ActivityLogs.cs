using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ActivityLogs
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string ActivityType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? EntityType { get; set; }

    public Guid? EntityId { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime OccurredAt { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users User { get; set; } = null!;
}
