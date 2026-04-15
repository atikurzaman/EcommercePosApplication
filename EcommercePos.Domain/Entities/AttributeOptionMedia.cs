using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class AttributeOptionMedia
{
    public Guid Id { get; set; }

    public Guid OptionId { get; set; }

    public Guid ProductId { get; set; }

    public string FileName { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public int FileSizeBytes { get; set; }

    public int? WidthPx { get; set; }

    public int? HeightPx { get; set; }

    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }

    public string? Etag { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual AttributeOptionMediaBlob? AttributeOptionMediaBlob { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual AttributeOptions Option { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
