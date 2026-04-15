using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class MediaAssets
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    public string? OriginalName { get; set; }

    public string? AltText { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? DurationSeconds { get; set; }

    public string StorageProvider { get; set; } = null!;

    public Guid? UploadedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? UploadedByNavigation { get; set; }
}
