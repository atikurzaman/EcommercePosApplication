using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Warehouses
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string SiteType { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public string? ContactPerson { get; set; }

    public string? ManagerName { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? Area { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string Country { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public TimeOnly? OpeningTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }

    public string? TaxNumber { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<CashShifts> CashShifts { get; set; } = new List<CashShifts>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<DayEndSummaries> DayEndSummaries { get; set; } = new List<DayEndSummaries>();

    public virtual ICollection<Employees> Employees { get; set; } = new List<Employees>();

    public virtual ICollection<Expenses> Expenses { get; set; } = new List<Expenses>();

    public virtual ICollection<GoodsReceipts> GoodsReceipts { get; set; } = new List<GoodsReceipts>();

    public virtual ICollection<InventoryAdjustments> InventoryAdjustments { get; set; } = new List<InventoryAdjustments>();

    public virtual ICollection<Warehouses> InverseParent { get; set; } = new List<Warehouses>();

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

    public virtual Warehouses? Parent { get; set; }

    public virtual ICollection<PickupPoints> PickupPoints { get; set; } = new List<PickupPoints>();

    public virtual ICollection<PosCounters> PosCounters { get; set; } = new List<PosCounters>();

    public virtual ICollection<PosTransactionReturns> PosTransactionReturns { get; set; } = new List<PosTransactionReturns>();

    public virtual ICollection<PosTransactions> PosTransactions { get; set; } = new List<PosTransactions>();

    public virtual ICollection<PurchaseOrders> PurchaseOrders { get; set; } = new List<PurchaseOrders>();

    public virtual ICollection<PurchaseReturns> PurchaseReturns { get; set; } = new List<PurchaseReturns>();

    public virtual ICollection<Quotes> Quotes { get; set; } = new List<Quotes>();

    public virtual ICollection<ReorderRules> ReorderRules { get; set; } = new List<ReorderRules>();

    public virtual ICollection<Shipments> Shipments { get; set; } = new List<Shipments>();

    public virtual ICollection<StockItems> StockItems { get; set; } = new List<StockItems>();

    public virtual ICollection<StockMovements> StockMovementsFromWarehouse { get; set; } = new List<StockMovements>();

    public virtual ICollection<StockMovements> StockMovementsToWarehouse { get; set; } = new List<StockMovements>();

    public virtual ICollection<StockTransfers> StockTransfersFromWarehouse { get; set; } = new List<StockTransfers>();

    public virtual ICollection<StockTransfers> StockTransfersToWarehouse { get; set; } = new List<StockTransfers>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
