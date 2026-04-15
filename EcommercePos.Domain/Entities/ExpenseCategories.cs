using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ExpenseCategories : AuditableEntity<Guid>
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual ICollection<Expenses> Expenses { get; set; } = new List<Expenses>();

    public virtual Users? UpdatedByNavigation { get; set; }
}
