using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class SupportTickets : AuditableEntity<Guid>
{
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
    public virtual Users? AssignedTo { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Orders? Order { get; set; }

    public virtual ICollection<SupportTicketMessages> SupportTicketMessages { get; set; } = new List<SupportTicketMessages>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }
}
