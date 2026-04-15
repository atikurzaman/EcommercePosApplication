using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class ProductMedia : AuditableEntity<Guid>
{
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
    public virtual Users? CreatedByNavigation { get; set; }

    public virtual Products Product { get; set; } = null!;

    public virtual ProductMediaBlob? ProductMediaBlob { get; set; }

    public virtual Users? UpdatedByNavigation { get; set; }

    public virtual Users? UploadedByNavigation { get; set; }

    public virtual ProductVariants? Variant { get; set; }
}
