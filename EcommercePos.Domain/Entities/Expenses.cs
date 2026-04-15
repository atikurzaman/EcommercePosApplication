using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Expenses
{
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid? ExpenseCategoryId { get; set; }

    public DateTime ExpenseDate { get; set; }

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public string? MethodCode { get; set; }

    public string? ReceiptReference { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? CreatedByUser { get; set; }

    public virtual ExpenseCategories? ExpenseCategory { get; set; }

    public virtual PaymentMethods? MethodCodeNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Warehouses Warehouse { get; set; } = null!;
}
