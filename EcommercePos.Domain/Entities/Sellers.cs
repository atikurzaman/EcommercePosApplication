using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Sellers : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }

    public string StoreName { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? StoreDescription { get; set; }

    public string? StoreLogo { get; set; }

    public string? StoreBanner { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? AddressLine1 { get; set; }

    public string? City { get; set; }

    public string Country { get; set; } = null!;

    public decimal Balance { get; set; }

    public decimal CommissionRate { get; set; }

    public bool IsApproved { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public bool IsActive { get; set; }
    public virtual Users? ApprovedByUser { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users User { get; set; } = null!;
}
