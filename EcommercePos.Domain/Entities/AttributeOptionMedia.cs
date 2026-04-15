using System;
using System.Collections.Generic;
using EcommercePos.Domain.Common;

namespace EcommercePos.Domain.Entities;

public partial class AttributeOptionMedia : AuditableEntity<Guid>
{
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
    public virtual AttributeOptionMediaBlob? AttributeOptionMediaBlob { get; set; }

    public virtual Users? CreatedByNavigation { get; set; }

    public virtual AttributeOptions Option { get; set; } = null!;

    public virtual Products Product { get; set; } = null!;

    public virtual Users? UpdatedByNavigation { get; set; }
}
