using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class Notifications
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Link { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public bool IsSent { get; set; }

    public DateTime? SentAt { get; set; }

    public string? TargetRole { get; set; }

    public bool SendEmail { get; set; }

    public bool SendSms { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? User { get; set; }
}
