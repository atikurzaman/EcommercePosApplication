using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class PosTransactionReturns : AuditableEntity<Guid>
{
    public string ReturnNo { get; set; } = null!;

    public DateTime ReturnDate { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid? CustomerId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public Guid? SaleId { get; set; }

    public Guid? CreatedByUserId { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? CreatedByUser { get; set; }

    public virtual Customers? Customer { get; set; }

    public virtual ICollection<PosTransactionReturnLines> PosTransactionReturnLines { get; set; } = new List<PosTransactionReturnLines>();

    public virtual PosTransactions? Sale { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
