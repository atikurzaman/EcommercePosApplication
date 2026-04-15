using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class Employees : AuditableEntity<Guid>
{
    public Guid? WarehouseId { get; set; }

    public Guid? UserId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? AddressLine1 { get; set; }

    public string? City { get; set; }

    public DateTime? JoiningDate { get; set; }

    public DateTime? TerminationDate { get; set; }

    public string? Designation { get; set; }

    public string? Department { get; set; }

    public string? EmployeeType { get; set; }

    public decimal? Salary { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? NationalId { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public string? PhotoUrl { get; set; }

    public string? ShiftPattern { get; set; }

    public bool IsActive { get; set; }
    public virtual ICollection<CashShifts> CashShiftsClosedByEmployee { get; set; } = new List<CashShifts>();

    public virtual ICollection<CashShifts> CashShiftsOpenedByEmployee { get; set; } = new List<CashShifts>();

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<PosTransactions> PosTransactions { get; set; } = new List<PosTransactions>();

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? User { get; set; }

    public virtual Warehouses? Warehouse { get; set; }
}
