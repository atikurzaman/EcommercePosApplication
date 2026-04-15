using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Quotes : AuditableEntity<Guid>
{
    public string QuoteNo { get; set; } = null!;

    public Guid? CustomerId { get; set; }

    public Guid WarehouseId { get; set; }

    public DateTime QuoteDate { get; set; }

    public DateTime? ValidUntilDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal GrandTotal { get; set; }

    public string Status { get; set; } = null!;

    public Guid? OrderId { get; set; }

    public string? Notes { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers? Customer { get; set; }

    public virtual Orders? Order { get; set; }

    public virtual ICollection<QuoteItems> QuoteItems { get; set; } = new List<QuoteItems>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
