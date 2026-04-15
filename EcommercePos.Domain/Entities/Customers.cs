using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class Customers
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string CustomerCode { get; set; } = null!;

    public string CustomerType { get; set; } = null!;

    public string? Phone { get; set; }

    public string? AlternatePhone { get; set; }

    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? CompanyName { get; set; }

    public string? TaxNumber { get; set; }

    public string? AddressLine1 { get; set; }

    public string? City { get; set; }

    public string Country { get; set; } = null!;

    public decimal Balance { get; set; }

    public decimal? CreditLimit { get; set; }

    public int LoyaltyPoints { get; set; }

    public string? CustomerGroup { get; set; }

    public string? ReferralCode { get; set; }

    public Guid? ReferredByCustomerId { get; set; }

    public DateTime RegistrationDate { get; set; }

    public DateTime? LastPurchaseDate { get; set; }

    public bool IsActive { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual ICollection<Carts> Carts { get; set; } = new List<Carts>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<CustomerAddresses> CustomerAddresses { get; set; } = new List<CustomerAddresses>();

    public virtual CustomerProfiles? CustomerProfiles { get; set; }

    public virtual CustomerWallets? CustomerWallets { get; set; }

    public virtual ICollection<DiscountUsageLog> DiscountUsageLog { get; set; } = new List<DiscountUsageLog>();

    public virtual ICollection<Customers> InverseReferredByCustomer { get; set; } = new List<Customers>();

    public virtual ICollection<LoyaltyTransactions> LoyaltyTransactions { get; set; } = new List<LoyaltyTransactions>();

    public virtual ICollection<NewsletterSubscribers> NewsletterSubscribers { get; set; } = new List<NewsletterSubscribers>();

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

    public virtual ICollection<PosTransactionReturns> PosTransactionReturns { get; set; } = new List<PosTransactionReturns>();

    public virtual ICollection<PosTransactions> PosTransactions { get; set; } = new List<PosTransactions>();

    public virtual ICollection<Quotes> Quotes { get; set; } = new List<Quotes>();

    public virtual Customers? ReferredByCustomer { get; set; }

    public virtual ICollection<RefundRequests> RefundRequests { get; set; } = new List<RefundRequests>();

    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }

    public virtual ICollection<Wishlists> Wishlists { get; set; } = new List<Wishlists>();
}
