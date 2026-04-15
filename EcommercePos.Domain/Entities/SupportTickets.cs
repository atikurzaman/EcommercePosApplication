using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class SupportTickets
{
    public Guid Id { get; set; }

    public string TicketNumber { get; set; } = null!;

    public Guid? UserId { get; set; }

    public Guid? AssignedToId { get; set; }

    public Guid? OrderId { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Category { get; set; }

    public string Priority { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? AdminNote { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? AssignedTo { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Orders? Order { get; set; }

    public virtual ICollection<SupportTicketMessages> SupportTicketMessages { get; set; } = new List<SupportTicketMessages>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }
}
