using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class SupportTicketMessages
{
    public Guid Id { get; set; }

    public Guid SupportTicketId { get; set; }

    public Guid? SenderId { get; set; }

    public string Message { get; set; } = null!;

    public bool IsFromAdmin { get; set; }

    public string? AttachmentUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? Sender { get; set; }

    public virtual SupportTickets SupportTicket { get; set; } = null!;
}
