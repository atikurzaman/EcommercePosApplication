using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ProductBatches : AuditableEntity<Guid>
{
    public Guid ProductId { get; set; }

    public string BatchNo { get; set; } = null!;

    public DateTime? ManufactureDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }
    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<GoodsReceiptLines> GoodsReceiptLines { get; set; } = new List<GoodsReceiptLines>();

    public virtual ICollection<InventoryAdjustmentLines> InventoryAdjustmentLines { get; set; } = new List<InventoryAdjustmentLines>();

    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

    public virtual ICollection<PosTransactionLines> PosTransactionLines { get; set; } = new List<PosTransactionLines>();

    public virtual ICollection<PosTransactionReturnLines> PosTransactionReturnLines { get; set; } = new List<PosTransactionReturnLines>();

    public virtual Products Product { get; set; } = null!;

    public virtual ICollection<PurchaseOrderLines> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLines>();

    public virtual ICollection<PurchaseReturnLines> PurchaseReturnLines { get; set; } = new List<PurchaseReturnLines>();

    public virtual ICollection<StockItems> StockItems { get; set; } = new List<StockItems>();

    public virtual ICollection<StockMovements> StockMovements { get; set; } = new List<StockMovements>();

    public virtual ICollection<StockTransferLines> StockTransferLines { get; set; } = new List<StockTransferLines>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
