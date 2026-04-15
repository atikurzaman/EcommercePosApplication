using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class CustomerAddresses : AuditableEntity<Guid>
{
    public Guid CustomerId { get; set; }

    public Guid? UserId { get; set; }

    public string AddressType { get; set; } = null!;

    public string? Label { get; set; }

    public string FullName { get; set; } = null!;

    public string? CompanyName { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string? AlternatePhone { get; set; }

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string Country { get; set; } = null!;

    public bool IsDefault { get; set; }

    public string? DeliveryInstructions { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Customers Customer { get; set; } = null!;

    public virtual ICollection<Orders> OrdersBillingAddress { get; set; } = new List<Orders>();

    public virtual ICollection<Orders> OrdersShippingAddress { get; set; } = new List<Orders>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }
}
