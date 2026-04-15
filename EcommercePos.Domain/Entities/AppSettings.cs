using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class AppSettings
{
    public Guid Id { get; set; }

    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public string Category { get; set; } = null!;

    public string DataType { get; set; } = null!;

    public bool IsPublic { get; set; }

    public bool IsEncrypted { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }
}
