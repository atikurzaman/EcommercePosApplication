using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Suppliers : AuditableEntity<Guid>
{
    public string SupplierCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? CompanyName { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public string? AlternatePhone { get; set; }

    public string? Email { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string Country { get; set; } = null!;

    public string? SupplierType { get; set; }

    public string? TaxRegistrationNo { get; set; }

    public string? PaymentTerms { get; set; }

    public int? LeadTimeDays { get; set; }

    public decimal Balance { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<ProductSupplierLinks> ProductSupplierLinks { get; set; } = new List<ProductSupplierLinks>();

    public virtual ICollection<PurchaseOrders> PurchaseOrders { get; set; } = new List<PurchaseOrders>();

    public virtual ICollection<PurchaseReturns> PurchaseReturns { get; set; } = new List<PurchaseReturns>();

    public virtual ICollection<ReorderRules> ReorderRules { get; set; } = new List<ReorderRules>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
