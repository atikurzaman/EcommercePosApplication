using System;
using System.Collections.Generic;

namespace EcommercePos.Persistence.Data;

public partial class ProductMedia
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid? VariantId { get; set; }

    public string Scope { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public int FileSizeBytes { get; set; }

    public int? WidthPx { get; set; }

    public int? HeightPx { get; set; }

    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }

    public string? Etag { get; set; }

    public Guid? UploadedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public byte[]? RowVersion { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ProductMediaBlob? ProductMediaBlob { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? UploadedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
