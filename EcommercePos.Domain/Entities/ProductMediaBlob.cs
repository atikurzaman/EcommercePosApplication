using System;
using System.Collections.Generic;

namespace EcommercePos.Domain.Entities;

public partial class ProductMediaBlob
{
    public Guid MediaId { get; set; }

    public byte[] Data { get; set; } = null!;

    public byte[]? ThumbnailData { get; set; }

    public byte[]? WebpData { get; set; }

    public virtual ProductMedia Media { get; set; } = null!;
}
