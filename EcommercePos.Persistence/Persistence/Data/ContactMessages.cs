using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class ContactMessages
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public bool IsReplied { get; set; }

    public string? ReplyMessage { get; set; }

    public DateTime? RepliedAt { get; set; }

    public Guid? RepliedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? RepliedByNavigation { get; set; }
}
